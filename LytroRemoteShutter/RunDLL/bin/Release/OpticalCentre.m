clear all
close all

%% Import data

% import UR5 pose data (X,Y,Z,ax,ay,az)
fid = fopen('UR5Trajectory/realsenseData/Matlab/UR5PoseData.txt');
tline = fgets(fid);
Gripper = [];
while ischar(tline)
    coord = zeros(1,6);
    coord = str2double(strsplit(tline,','));
    Gripper(end + 1,:) = coord;
    tline = fgets(fid);
end
fclose(fid);


% import LF Toolbox calibrated pose data (X,Y,Z)
fid = fopen('LFToolbox0.4/LFToolbox0.3_Samples1/LFPoseData4.txt');
tline = fgets(fid);
coord = [];
while ischar(tline)
    coord(end+1,:) = str2double(strsplit(tline,','));
    tline = fgets(fid);
end
fclose(fid);

coord2 = coord(2:2:end,:);
coord(2:2:end,:) = [];
X = coord(:,1);
Y = coord(:,3);
Z = coord2(:,2);

LF = [X Y Z];

%% Transform Lytro Coordinates

R = [0 0 -1;
    1 0 0;
    0 -1 0];

T = [-1.63;-LF(1,1);0.1];

LF2World = [];
for r = 1:length(LF)
    
    LF2World(end+1,:)= (R*[LF(r,1);LF(r,2);LF(r,3)]+T)';

end
LF2World = [LF2World(1,:); LF2World(12,:); LF2World(23,:); LF2World(34,:); 
    LF2World(45:end,:); LF2World(2:11,:); LF2World(13:22,:);
    LF2World(24:33,:); LF2World(35:44,:);];

for i = 1:length(Gripper)
    
    distance(i) = norm(LF2World(i,:)-Gripper(i,1:3));
    
end
offset = sum(distance)/length(distance);

%% Apply offset

OC = [];
for i = 1:length(Gripper)
    
    off = (rot([Gripper(i,4); Gripper(i,5); Gripper(i,6)])*[0;0;offset])';
    
    OC(end+1,:) = (Gripper(i,1:3) + off);
        
end

%% Plot

figure
e = [];
hold on
axis square
grid on
plot3(LF2World(:,1),LF2World(:,2), LF2World(:,3),'.-');
plot3(Gripper(:,1),Gripper(:,2),Gripper(:,3),'+-');
plot3(OC(:,1),OC(:,2),OC(:,3),'k.');
for i = 1:length(Gripper)
    %line([LF2World(i,1) Gripper(i,1)],[LF2World(i,2) Gripper(i,2)],[LF2World(i,3) Gripper(i,3)]);
    %line([OC(i,1) Gripper(i,1)],[OC(i,2) Gripper(i,2)],[OC(i,3) Gripper(i,3)],'Color','k');
    e(end+1) = norm(LF2World(i,:) - OC(i,:)); % error calc
end
plot3(-1.7,0,0,'r*');
plot3(T(1,1),T(2,1),-T(3,1),'k-');
plot3(T(1,1),0.1230,-T(3,1),'k-');
plot3(T(1,1),0.1230,-T(3,1)+0.19,'k-');
plot3(T(1,1),T(2,1),-T(3,1)+0.19,'k-');
line([T(1,1),T(1,1)],[T(2,1),0.1230],[-T(3,1),-T(3,1)],'Color','k');
line([T(1,1),T(1,1)],[T(2,1),T(2,1)],[-T(3,1),-T(3,1)+0.19],'Color','k');
line([T(1,1),T(1,1)],[0.1230,0.1230],[-T(3,1),-T(3,1)+0.19],'Color','k');
line([T(1,1),T(1,1)],[T(2,1),0.1230],[-T(3,1)+0.19,-T(3,1)+0.19],'Color','k');


xlabel('x (m)');
ylabel('y (m)');
zlabel('z (m)');
legend('Lytro','UR5','X_e');

string = "The mean error is: " +num2str((sum(e)/length(e))*1000) +" mm";
disp(string);


    

