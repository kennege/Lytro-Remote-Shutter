function [syncedPoses] = syncPoseData(dataFilePath,poseFormat,poseParameterisation)
%SYNCPOSEDATA outputs array of poses and indexes of corresponding rgb and
%depth images
%each row = [time,pose (x,y,z,scaled axis), rgbImage no., depthImage no.]


rawData = load(strcat(dataFilePath,'poseData.txt'));

switch poseFormat
    case 'UR5'
        syncedPoses = zeros(size(rawData,1),9);
        syncedPoses(:,1:7) = rawData;
        syncedPoses(:,8:9) = repmat([1:size(rawData,1)]',1,2);
    case 'Vicon'
        %image times - [imageNo,secs,nsecs]
        rgbImageTimes   = load(strcat(dataFilePath,'rgbTimes.txt'));
        rgbImageTimes   = [rgbImageTimes(:,1),rgbImageTimes(:,2)+1e-9*rgbImageTimes(:,3)];
        depthImageTimes = load(strcat(dataFilePath,'depthTimes.txt'));
        depthImageTimes = [depthImageTimes(:,1),depthImageTimes(:,2)+1e-9*depthImageTimes(:,3)];

        %raw pose data
        rawData(:,2) = rawData(:,2) + 1e-9*rawData(:,3);
        rawData(:,[1 3]) = [];
        
        %clip empty rows
        rawData(rawData(:,1)==0,:) = [];

        %adjust time
        t1 = rgbImageTimes(1,2);
        rgbImageTimes(:,2) = rgbImageTimes(:,2) - t1;
        depthImageTimes(:,2) = depthImageTimes(:,2) - t1;
        rawData(:,1) = rawData(:,1) - t1;
        rawData(:,1) = rawData(:,1) - rawData(1,1); %time wrong - start together
        rawData = rawData(:,[1:4 8 5 6 7]); %reorder quaternions

        %frames where pose is required
        imageTimes = union(rgbImageTimes(:,2),depthImageTimes(:,2));
        %remove frame times before and after pose times
        imageTimes(imageTimes<rawData(1,1)|imageTimes>rawData(end,1)) = [];
        nFrames = numel(imageTimes);
        %interpolate - get poses at frameTimes
        poseDataInterp = zeros(nFrames,8); %[t,pos,quat]
        poseDataInterp(:,1) = imageTimes;
        poseDataInterp(:,2:4) = interp1(rawData(:,1),rawData(:,2:4),imageTimes);
        poseDataInterp(:,5:8) = interp1Quat(rawData(:,1),rawData(:,5:8),imageTimes);
        poseDataInterpAxis = zeros(nFrames,7);
        poseDataInterpAxis(:,1:4) = poseDataInterp(:,1:4);
        for i = 1:nFrames
            poseDataInterpAxis(i,5:7) = q2a(poseDataInterp(i,5:8));
        end

        %store synced data - row = [t,pose,rgbFrame,depthFrame]
        syncedPoses = zeros(nFrames,9); 
        syncedPoses(:,1:7) = poseDataInterpAxis;

        %this is slow, but works in case of missed frames
        for i = 1:nFrames
            if ~isempty(rgbImageTimes(rgbImageTimes(:,2)==imageTimes(i),1))
                syncedPoses(i,8) = rgbImageTimes(rgbImageTimes(:,2)==imageTimes(i),1);
            end
            if ~isempty(depthImageTimes(depthImageTimes(:,2)==imageTimes(i),1))
                syncedPoses(i,9) = depthImageTimes(depthImageTimes(:,2)==imageTimes(i),1);
            end
        end
        %remove times with no image
        syncedPoses(any(syncedPoses(:,8:9)==0,2),:) = [];             
        
end

% convert to SE3
if strcmp(poseParameterisation,'SE3')
    for i = 1:size(syncedPoses,1)
        syncedPoses(i,2:7) = R3xso3_LogSE3(syncedPoses(i,2:7)')';
    end
end

end

