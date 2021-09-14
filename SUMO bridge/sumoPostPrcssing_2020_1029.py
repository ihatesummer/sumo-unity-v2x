import sys
import math
import datetime
from typing import List
from abc import ABCMeta, abstractmethod

import pandas as pd
import scipy.io as sio
from tqdm import tqdm
import xml.etree.ElementTree as elemTree


file_1 = 'sumoTrace'
traceFileList = [file_1]
cntFiles = 0


class Trace(metaclass=ABCMeta):
    @abstractmethod
    def extract_mobility(self, trace_file_name: str) -> pd.DataFrame:
        pass


class PersonTrace(Trace):
    def __init__(self):
        self.personList: List = []

    def extract_mobility(self, trace_file_name: str) -> pd.DataFrame:
        for _, sumoPreProc in tqdm(elemTree.iterparse(trace_file_name+'.xml')):
            if sumoPreProc.tag == "person":
                self.personList.append([sumoPreProc.attrib['id'], sumoPreProc.attrib['x'], sumoPreProc.attrib['y']])

            # if size of person list exceed 25MB, write to xml
            if sys.getsizeof(self.personList) >= 25_000_000:
                print(f"Finish: reading sumo trace file of {trace_file_name} 25MB @{datetime.datetime.now()}")
                print(f"Start:  writing sumo trace file of {trace_file_name} 25MB @{datetime.datetime.now()}")
                sumo_post_proc = Converter.to_data_frame(self.personList, ['id', 'x', 'y'])
                FileWriter.write_csv(sumo_post_proc, trace_file_name)
                print(f"Finish: writing sumo trace file of {trace_file_name} 25MB @{datetime.datetime.now()}")
                self.personList.clear()

            sumoPreProc.clear()

        sumo_post_proc = Converter.to_data_frame(self.personList, ['id', 'x', 'y'])
        print(f"post processing for sumo trace file, {trace_file_name} has completed @{datetime.datetime.now()}")
        return sumo_post_proc


class VehicleTrace(Trace):
    def __init__(self):
        self.vehicleList: List = []

    def extract_mobility(self, trace_file_name) -> pd.DataFrame:
        time: str = ""
        for event, sumoPreProc in tqdm(elemTree.iterparse(trace_file_name + '.xml', events=("start", ))):
            if event == "start" and sumoPreProc.tag == "timestep":
                time = sumoPreProc.attrib['time']
            if event == "start" and sumoPreProc.tag == "vehicle":
                self.vehicleList.append([time, sumoPreProc.attrib['id'], sumoPreProc.attrib['x'], sumoPreProc.attrib['y']])

            # if size of person list exceed 25MB, write to xml
            if sys.getsizeof(self.vehicleList) >= 25_000_000:
                print(f"Finish: reading sumo trace file of {trace_file_name} 25MB @{datetime.datetime.now()}")
                print(f"Start:  writing sumo trace file of {trace_file_name} 25MB @{datetime.datetime.now()}")
                sumo_post_proc = Converter.to_data_frame(self.vehicleList, ['time', 'id', 'x', 'y'])
                FileWriter.write_csv(sumo_post_proc, trace_file_name)
                print(f"Finish: writing sumo trace file of {trace_file_name} 25MB @{datetime.datetime.now()}")
                self.vehicleList.clear()

            sumoPreProc.clear()

        sumo_post_proc = Converter.to_data_frame(self.vehicleList, ['time', 'id', 'x', 'y'])
        print(f"post processing for sumo trace file, {trace_file_name} has completed @{datetime.datetime.now()}")
        return sumo_post_proc


class Converter:

    @staticmethod
    def to_data_frame(target: List, column_names: List) -> pd.DataFrame:
        data_frame = pd.DataFrame(target, columns=column_names)
        print(len(data_frame['id']))
        data_frame['id'] = data_frame['id'].str.slice(start=3)
        sorted_data_frame = data_frame.sort_values(by=['id'], axis=0, ascending=True, inplace=False, kind='mergesort')
        if "time" in column_names:
            sorted_data_frame['time'] = pd.to_numeric(sorted_data_frame['time'])
        sorted_data_frame['id'] = pd.to_numeric(sorted_data_frame['id'])
        sorted_data_frame['x'] = pd.to_numeric(sorted_data_frame['x'])
        sorted_data_frame['y'] = pd.to_numeric(sorted_data_frame['y'])
        return sorted_data_frame


class FileWriter:

    @staticmethod
    def write_mat(sumo_post_proc: pd.DataFrame, output_file_name: str):
        print("file I/O : to_mat")
        global cntFiles
        sumo_post_proc = sumo_post_proc.to_dict()
        sio.savemat(output_file_name+'_'+str(cntFiles), sumo_post_proc)
        cntFiles = cntFiles + 1

    @staticmethod
    def write_csv(sumo_post_proc: pd.DataFrame, output_file_name: str):
        print("file I/O : to_csv")
        global cntFiles
        sumo_post_proc.to_csv(
            output_file_name+'_'+str(cntFiles)+'.csv', sep=' ', na_rep='NaN', chunksize=1_000_000, index=False,
            header=False, encoding='utf-8', mode='a')
        cntFiles = cntFiles + 1

    @staticmethod
    def write_xlsx(sumo_post_proc: pd.DataFrame, output_file_name: str):
        print("file I/O : to_excel")
        global cntFiles
        millions = math.ceil(len(sumo_post_proc['id']) / 1000000)
        writer = pd.ExcelWriter(output_file_name+'_'+str(cntFiles)+'.xlsx')
        for million in tqdm(range(millions)):
            if million % 2 == 0 and million != 0:
                writer.close()
                cntFiles = cntFiles + 1
                writer = pd.ExcelWriter(output_file_name+'_'+str(cntFiles)+'.xlsx')
            sumo_post_proc[million*1000000:million*1000000+1000000].to_excel(
                writer, sheet_name='Sheet{everyMillionLines}'.format(everyMillionLines=million), index=False)
        writer.close()


def main():
    global cntFiles
    print("sumo trace file post processing @ "+str(datetime.datetime.now()))
    vehicle = VehicleTrace()

    for files in traceFileList:
        sumo_post_proc = vehicle.extract_mobility(files)
        FileWriter.write_csv(sumo_post_proc, files)
        cntFiles = 0


if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        print("Processing has been stopped")
