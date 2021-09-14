import time
import matplotlib.pyplot as plt
from tcp_server import TcpServer
from sumo_python_bridge import SumoClient
from data_processor import DataProcessor
from python_unity_bridge import UnityBridge
from PlotPerformanceMetrics import PerformanceMetrics

SUMO_DIRECTORY = "2021-09-14-14-21-47"


def init_program(sumo_dir=SUMO_DIRECTORY, ip='127.0.0.1', port=4042):
    sumo_client = SumoClient(sumo_dir, "osm.sumocfg")
    sumo_client.start_sumo()
    server = TcpServer(ip, port)
    unity_client = server.boot()
    print(f'Connected to Unity TCP Client : {server.wait_unity()}')

    unity = UnityBridge()
    unity.start_unity(unity_client)

    data_processor = DataProcessor()

    vehicles_id, traffic_lights_id = sumo_client.get_all_list()
    veh_obj, tls_obj = data_processor.parse_obj(vehicles_id, traffic_lights_id)
    return sumo_client, server, data_processor, veh_obj, tls_obj, unity


def main():
    global step_cnt, veh_obj, tls_obj
    sumo_client, server, data_processor,\
        veh_obj, tls_obj, unity\
        = init_program(sumo_dir=SUMO_DIRECTORY)
    unity.construct_msg(veh_obj, tls_obj)
    # plt, fig, performance_metric,\
    #     line_1, line_2, line_3, line_4, line_5, line_6,\
    #     axes_1, axes_2, axes_3, axes_4, axes_5, axes_6\
    #     = plotMetric()
    while step_cnt < MAX_STEP:
        if unity._queue.empty():
            #  print(f'main running; current step = {step_cnt}')
            running(sumo_client, data_processor, unity)
        # if unity.recv_queue.not_empty:
        #     performance_metric.extract_metrics(unity.recv_queue)
            # for line, axes, error_queue in zip(
            #     [line_1, line_2, line_3, line_4, line_5, line_6],
            #     [axes_1, axes_2, axes_3, axes_4, axes_5, axes_6],
            #         [performance_metric.absolute_error_min,
            #          performance_metric.absolute_error_max,
            #          performance_metric.absolute_error_average,
            #          performance_metric.relative_error_min,
            #          performance_metric.relative_error_max,
            #          performance_metric.relative_error_average]):
            #     y_data = list(error_queue.queue)
            #     line.set_ydata(y_data)
            #     line.set_xdata(list(range(len(y_data))))
            #     axes.set_xlim(0, len(y_data)-1)

        plt.draw()
        plt.pause(0.1)
        time.sleep(0.1)
        step_cnt += 1
    close(sumo_client, unity, server)
    plt.show(block=True)


def running(sumo_client, data_processor, unity):
    global veh_obj, tls_obj
    sumo_client.next_step()
    vehicles_id, _ = sumo_client.get_all_list()
    veh_obj, tls_obj = data_processor.update_obj(veh_obj, vehicles_id, tls_obj)
    unity.construct_msg(veh_obj, tls_obj)
    unity.recv_msg()
    time.sleep(0.1)


def plotMetric():
    performance_metric = PerformanceMetrics()
    y_limit = 5.0
    plt.ion()
    fig = plt.figure(figsize=(16, 8))
    axes_1 = fig.add_subplot(231)
    axes_1.set_ylim(0, y_limit)
    axes_1.set_ylabel("m")
    axes_1.set_title("absolute error : min")
    line_1, = axes_1.plot([], [])
    axes_2 = fig.add_subplot(232)
    axes_2.set_ylim(0, y_limit)
    axes_2.set_ylabel("m")
    axes_2.set_title("absolute error : max")
    line_2, = axes_2.plot([], [])
    axes_3 = fig.add_subplot(233)
    axes_3.set_ylim(0, y_limit)
    axes_3.set_ylabel("m")
    axes_3.set_title("absolute error : average")
    line_3, = axes_3.plot([], [])
    axes_4 = fig.add_subplot(234)
    axes_4.set_ylim(0, y_limit)
    axes_4.set_ylabel("m")
    axes_4.set_title("relative error : min")
    line_4, = axes_4.plot([], [])
    axes_5 = fig.add_subplot(235)
    axes_5.set_ylim(0, y_limit)
    axes_5.set_ylabel("m")
    axes_5.set_title("relative error : max")
    line_5, = axes_5.plot([], [])
    axes_6 = fig.add_subplot(236)
    axes_6.set_ylim(0, y_limit)
    axes_6.set_ylabel("m")
    axes_6.set_title("relative error : average")
    line_6, = axes_6.plot([], [])
    return plt, fig, performance_metric,\
        line_1, line_2, line_3, line_4, line_5, line_6,\
        axes_1, axes_2, axes_3, axes_4, axes_5, axes_6


def close(sumo_client, unity, server):
    unity.is_running = False
    server.send_msg('finish')
    sumo_client.close()
    server.shut_down()


if __name__ == '__main__':
    MAX_STEP = 3826
    step_cnt = 0
    veh_obj, tls_obj = None, None
    main()
