function generatePoseData(poses,fileName)
%GENERATEPOSEDATA generates polyscope script with list of waypoints

%% 1. open file
if ispc
    sep = '\';
elseif isunix || ismac
    sep = '/';
end
fileID = fopen(strcat(pwd,sep,'UR5PoseData',sep,'poseData.txt'),'w');

%% 2. write poses
for i = 1:size(poses,2)
    formatSpec = strcat('%d',repmat(' %.6f',1,6),'\n');
    fprintf(fileID,formatSpec,i,poses(:,i)');
end

%% 5. close files
fclose(fileID);

end

