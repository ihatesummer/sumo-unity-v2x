from queue import Queue
import re

from typing import List, Tuple


class PerformanceMetrics:
    def __init__(self):
        self.regex_float: re = re.compile(r'\d[.]?\d*')
        self.regex_abs_min: re = re.compile(r'Absolute Error \(min\)\s\d[.]?\d*\s')
        self.regex_abs_max: re = re.compile(r'Absolute Error \(max\)\s\d[.]?\d*\s')
        self.regex_abs_avg: re = re.compile(r'Absolute Error \(avg\)\s\d[.]?\d*\s')
        self.regex_rel_min: re = re.compile(r'Relative Error \(min\)\s\d[.]?\d*\s')
        self.regex_rel_max: re = re.compile(r'Relative Error \(max\)\s\d[.]?\d*\s')
        self.regex_rel_avg: re = re.compile(r'Relative Error \(avg\)\s\d[.]?\d*\s')
        self.absolute_error_min: Queue = Queue()
        self.absolute_error_max: Queue = Queue()
        self.absolute_error_average: Queue = Queue()
        self.relative_error_min: Queue = Queue()
        self.relative_error_max: Queue = Queue()
        self.relative_error_average: Queue = Queue()
        #self.tot_absolute_error_min = []
        #self.tot_absolute_error_max = []
        #self.tot_absolute_error_average = []
        #self.tot_relative_error_min = []
        #self.tot_relative_error_max = []
        #self.tot_relative_error_average = []
        #self.num_samples = []

    def extract_metrics(self, recv_queue: Queue) -> None:
        if not recv_queue.empty():
            search_target: str = "".join(list(recv_queue.get()))
            #search_result = re.findall(self.regex_float, search_target)
            #self.absolute_error_min.put(float(search_result[0]))
            #self.absolute_error_max.put(float(search_result[0]))
            #self.absolute_error_average.put(float(search_result[0]))
            #self.relative_error_min.put(float(search_result[0]))
            #self.relative_error_max.put(float(search_result[0]))
            #self.relative_error_average.put(float(search_result[0]))

            abs_min: str = re.findall(self.regex_abs_min, search_target)
            abs_max: str = re.findall(self.regex_abs_max, search_target)
            abs_avg: str = re.findall(self.regex_abs_avg, search_target)
            rel_min: str = re.findall(self.regex_rel_min, search_target)
            rel_max: str = re.findall(self.regex_rel_max, search_target)
            rel_avg: str = re.findall(self.regex_rel_avg, search_target)
            search_result_abs_min: List[str] = re.findall(self.regex_float, "".join(list(abs_min)))
            search_result_abs_max: List[str] = re.findall(self.regex_float, "".join(list(abs_max)))
            search_result_abs_avg: List[str] = re.findall(self.regex_float, "".join(list(abs_avg)))
            search_result_rel_min: List[str] = re.findall(self.regex_float, "".join(list(rel_min)))
            search_result_rel_max: List[str] = re.findall(self.regex_float, "".join(list(rel_max)))
            search_result_rel_avg: List[str] = re.findall(self.regex_float, "".join(list(rel_avg)))
            [self.absolute_error_min.put(float(err)) for err in search_result_abs_min]
            [self.absolute_error_max.put(float(err)) for err in search_result_abs_max]
            [self.absolute_error_average.put(float(err)) for err in search_result_abs_avg]
            [self.relative_error_min.put(float(err)) for err in search_result_rel_min]
            [self.relative_error_max.put(float(err)) for err in search_result_rel_max]
            [self.relative_error_average.put(float(err)) for err in search_result_rel_avg]

            #[self.tot_absolute_error_min.append(float(err)) for err in search_result_abs_min]
            #[self.tot_absolute_error_max.append(float(err)) for err in search_result_abs_max]
            #[self.tot_absolute_error_average.append(float(err)) for err in search_result_abs_avg]
            #[self.tot_relative_error_min.append(float(err)) for err in search_result_rel_min]
            #[self.tot_relative_error_max.append(float(err)) for err in search_result_rel_max]
            #[self.tot_relative_error_average.append(float(err)) for err in search_result_rel_avg]

    def get_abs_error_metrics(self) -> Tuple[Queue, Queue, Queue]:
        return self.absolute_error_min, self.absolute_error_max, self.absolute_error_average

    def get_rel_error_metrics(self) -> Tuple[Queue, Queue, Queue]:
        return self.relative_error_min, self.relative_error_max, self.relative_error_average


if __name__ == "__main__":
    pMetric = PerformanceMetrics()
    recv_q = Queue()
    recv_q.put('1')
    recv_q.put(' ')
    recv_q.put('2')
    recv_q.put(' ')
    recv_q.put('3')
    recv_q.put(' ')
    recv_q.put('4')
    recv_q.put(' ')
    recv_q.put('5')
    recv_q.put(' ')
    recv_q.put('6')
    recv_q.put(' ')
    pMetric.extract_metrics(recv_q)
    abs_min, abs_max, abs_avg = pMetric.get_abs_error_metrics()
    print(f'{list(abs_min.queue)} {list(abs_max.queue)} {list(abs_avg.queue)}')
