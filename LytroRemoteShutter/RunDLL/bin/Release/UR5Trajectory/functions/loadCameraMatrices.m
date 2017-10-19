function [rgbK,depthK,rgbP,depthP] = loadCameraMatrices(filePath)
%LOADCAMERAMATRICES Summary of this function goes here
%   Detailed explanation goes here

%loadcamera matrices
rgbFileID = fopen(strcat(filePath,'rgbInfo.txt'),'r');
rgbLine = fgets(rgbFileID); rgbLine = fgets(rgbFileID); rgbLine = fgets(rgbFileID);
rgbLineSplit = strsplit(rgbLine);
rgbK = reshape(cellfun(@str2num,{rgbLineSplit{5:13}}),3,3)';
rgbLine = fgets(rgbFileID);
rgbLineSplit = strsplit(rgbLine);
rgbP = reshape(cellfun(@str2num,{rgbLineSplit{4:15}}),4,3)';

depthFileID = fopen(strcat(filePath,'depthInfo.txt'),'r');
depthLine = fgets(depthFileID); depthLine = fgets(depthFileID); depthLine = fgets(depthFileID);
depthLineSplit = strsplit(depthLine);
depthK = reshape(cellfun(@str2num,{depthLineSplit{5:13}}),3,3)';
depthLine = fgets(depthFileID);
depthLineSplit = strsplit(depthLine);
depthP = reshape(cellfun(@str2num,{depthLineSplit{4:15}}),4,3)';

end

