import sys
from typing import List

import pandas as pd
import scipy.io as sio
import numpy as np
import math
from tqdm import tqdm
import xml.etree.ElementTree as elemTree
import xml.etree.cElementTree as celemTree

import datetime
from memory_profiler import profile

file_1 = 'Trace5-2'
file_2 = 'Trace5-3'
file_3 = 'Trace5-4'
traceFileList = [file_1, file_2, file_3]
cntFiles = 0


def procXML(traceFileName):
    personList = []
    for _, sumoPreProc in tqdm(celemTree.iterparse(traceFileName+'.xml')):
        if sumoPreProc.tag == "person":
            personList.append([sumoPreProc.attrib['id'], sumoPreProc.attrib['x'], sumoPreProc.attrib['y']])

        # if size of person list exceed 50MB, write to xmls
        if sys.getsizeof(personList) >= 25_000_000:
            print("Finish: reading sumo trace file of " + str(traceFileName) + " 25MB @ " + str(datetime.datetime.now()))
            print("Start:  writing sumo trace file of " + str(traceFileName) + " 25MB @ " + str(datetime.datetime.now()))
            sumoPostProc = to_data_frame(personList)
            writeCSV(sumoPostProc, traceFileName)
#            writeMAT(sumoPostProc, traceFileName)
            print("Finish: writing sumo trace file of " + str(traceFileName) + " 25MB @ " + str(datetime.datetime.now()))
            personList.clear()

        sumoPreProc.clear()

    sumoPostProc = to_data_frame(personList)
    print("post processing for sumo trace file, " + str(traceFileName) + " has completed @ "+str(datetime.datetime.now()))
    return sumoPostProc


def to_data_frame(from_list):
    dataFrame = pd.DataFrame(from_list, columns=['id', 'x', 'y'])
    print(len(dataFrame['id']))
    dataFrame['id'] = dataFrame['id'].str.slice(start=3)
    sorted_dataFrame = dataFrame.sort_values(by=['id'], axis=0, ascending=True, inplace=False, kind='mergesort')
    sorted_dataFrame['id'] = pd.to_numeric(sorted_dataFrame['id'])
    sorted_dataFrame['x'] = pd.to_numeric(sorted_dataFrame['x'])
    sorted_dataFrame['y'] = pd.to_numeric(sorted_dataFrame['y'])
    return sorted_dataFrame

def writeMAT(sumoPostProc, outputFileName):
    print("file I/O : to_mat")
    global cntFiles
    sumoPostProc = sumoPostProc.to_dict()
    sio.savemat(outputFileName+'_'+str(cntFiles), sumoPostProc)
    cntFiles = cntFiles + 1


def writeCSV(sumoPostProc, outputFileName):
    print("file I/O : to_csv")
    global cntFiles
    sumoPostProc.to_csv(
        outputFileName+'_'+str(cntFiles)+'.csv', sep=' ', na_rep='NaN', chunksize=1_000_000, index=False,
        header=False,encoding='utf-8', mode='a')
    cntFiles = cntFiles + 1


def writeXLSX(sumoPostProc, outputFileName):
    print("file I/O : to_excel")
    global cntFiles
    millions = math.ceil(len(sumoPostProc['id']) / 1000000)
    writer = pd.ExcelWriter(outputFileName+'_'+str(cntFiles)+'.xlsx')
    for million in tqdm(range(millions)):
        if million % 2 == 0 and million != 0:
            writer.close()
            cntFiles = cntFiles + 1
            writer = pd.ExcelWriter(outputFileName+'_'+str(cntFiles)+'.xlsx')
        sumoPostProc[million*1000000:million*1000000+1000000].to_excel(
            writer, sheet_name='Sheet{everyMillionLines}'.format(everyMillionLines=million), index=False)
    writer.close()


def main():
    global cntFiles
    print("sumo trace file post processing @ "+str(datetime.datetime.now()))
    for files in traceFileList:
        sumoPostProc = procXML(files)
        writeCSV(sumoPostProc, files)
#        writeMAT(sumoPostProc, files)
        cntFiles = 0

if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        print("Processing has been stopped")
