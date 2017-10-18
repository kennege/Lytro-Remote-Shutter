% Init the ethernet port on PC
% Author: Long Qian
% Date: June 2016

function s = init(ip,port)

    % Connect to robot
    Robot_IP = ip;
    Socket_conn = tcpip(Robot_IP,port,'NetworkRole','server');
    fclose(Socket_conn);
    disp('Press Play on Robot...')
    fopen(Socket_conn);
    disp('Connected!');
    
    s = Socket_conn;
end