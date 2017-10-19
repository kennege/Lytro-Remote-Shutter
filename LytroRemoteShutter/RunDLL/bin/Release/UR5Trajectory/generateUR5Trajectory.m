clear all
close all
addpath(genpath(pwd))

%% 1. Settings
nSteps = 50;
% velocity = 'constant';
velocity = 'variable';
createPolyscopeScript = 1;
generatePoseFile = 1;
fileName = 'abc_200';

%% 2. Compute t for desired velocity
switch velocity
    case 'variable'
        dt = 2*pi/nSteps;
        t = 0:dt:2*pi-dt;
    case 'constant'
        a = 0.15;
        b = 0.5;
        c = 0.15;
        f = @(x)((a*cos(x)).^2+(-3*b*sin(3*x)).^2+(2*c*cos(2*x)).^2).^(1/2);
        arcLength = integral(f,0,2*pi); 
        segmentLength = arcLength/nSteps;
        tol = 1e-10;
        dt = 0.1;
        t0 = 0;
        t = zeros(1,nSteps);
        for i = 1:nSteps-1
            done = 0;
            while ~done
                t1 = t0 + dt;
                error = segmentLength - integral(f,t0,t1);
                if abs(error) < tol
                    t(i+1) = t1;
                    t0 = t1;
                    done = 1;
                elseif error > 0
                    dt = dt + 0.5*abs(error);
                elseif error < 0
                    dt = dt - 0.5*abs(error);
                end           
            end
        end
end

%% 3. Poses
%ABC logo + sinusoidal Z
poses = zeros(6,nSteps);
poses(1,:) = 0.15*sin(2*t) - 0.45;
poses(2,:) = 0.5*sin(t) + 0;
poses(3,:) = 0.15*cos(3*t) + 0.45;
% poses(1,:) = -0.45;
% poses(2,:) = 0.1*t;
% poses(3,:) = 0.45;
% 
% poses(3,21:40) = 0.4;
% poses(2,21:40) = fliplr(poses(2,21:40)-poses(2,21));
% 
% poses(3,41:60) = 0.35;
% poses(2,41:60) = poses(2,41:60)-poses(2,41);
% 
% poses(3,61:80) = 0.3;
% poses(2,61:80) = fliplr(poses(2,61:80)-poses(2,61));
% 
% poses(3,81:100) = 0.25;
% poses(2,81:100) = poses(2,81:100)-poses(2,81);


%orient to face some focal point
focus = [-1.7,0,0]';
%focus = [-1.5,0,0]';
focus = repmat(focus,1,nSteps);
forward = [0,0,1]';

for i = 1:nSteps
    v = focus(:,i) - poses(1:3,i);
    %compute axis angle to go from forward-> v
    axisAngle = vrrotvec(forward,v);
    R1 = rot(unit(v)*pi/2); %reorient camera
    R2 = rot(axisAngle(1:3)'*axisAngle(4)); %face focus
    poses(4:6,i) = arot(R1*R2)'; 
    %poses(4:6,i) = [0 2.8*pi/2 0]; % ADDED
end


poses = [poses poses(:,1:10)];
%% 4. Generate polyscope script
if createPolyscopeScript
    generatePolyscopeScript(poses,fileName);
end
if generatePoseFile
    generatePoseData(poses,fileName);
end

%% 5. Plotting
fileID = fopen('UR5PoseData.txt','w'); % ADDED

figure
hold on
grid on
plot3(poses(1,:),poses(2,:),poses(3,:),'.')
for i = 1:nSteps
    fprintf(fileID,'%6.8f, %6.8f, %6.8f, %6.8f, %6.8f, %6.8f \n',poses(1,i),poses(2,i),poses(3,i),poses(4,i),poses(5,i),poses(6,i)); % ADDED
    plotiCamera = plotCamera('Location',poses(1:3,i),'Orientation',rot(-poses(4:6,i))); %LHS invert pose
    plotiCamera.Opacity = 0.1;
    plotiCamera.Size = 0.01;
    plotiCamera.Color = [0 0 0];
%     plotiCamera.AxesVisible = 1;
end
fclose(fileID); % ADDED
plot3(focus(1),focus(2),focus(3),'r*')
xlabel('x')
ylabel('y')
zlabel('z')
axis equal
view(-80,5)
