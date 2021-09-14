import pandas as pd
import numpy as np

def readXML():
    fpPostProc = pd.read_csv('./sumoTraceProc.csv', sep='"', header=None, encoding='utf-8')
    return fpPostProc

def procXML(sumoPreProc):
    sumoPostProc = sumoPreProc.drop([0, 2, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16], axis=1)
    sumoPostProc.columns = ['pedID', 'x', 'y']
    sumoPostProc = pd.DataFrame(sumoPostProc, dtype=float)
##    sumoPostProc.sort_values(by=['pedID'], axis=0, ascending=True)
    sumoPostProc = sumoPostProc.sort_values(by=['pedID'],axis=0, ascending=True, inplace=False, kind='mergesort')
    return sumoPostProc

def writeCSV(sumoPostProc):
    sumoPostProcArr = np.asarray(sumoPostProc, dtype=np.str)
    np.savetxt('./sumoTrace.csv', sumoPostProcArr, delimiter=' ', fmt='%s', encoding='utf-8')

def writeXLSX(sumoPostProc):
    writer = pd.ExcelWriter('sumoTrace.xlsx')
    sumoPostProc.to_excel(writer, sheet_name='Sheet1', index=False)
    writer.close()


def main():
    sumoPreProc = readXML()
    sumoPostProc = procXML(sumoPreProc)
    writeCSV(sumoPostProc)

if __name__ == '__main__':
    main()
