function [rgbImg,depthImg,bwImg] = getImages(filePath,rgbImgNo,depthImgNo)
%GETIMAGES Summary of this function goes here
%   Detailed explanation goes here

rgbFile = strcat('rgb_',num2str(rgbImgNo),'.png');
rgbImg = imread(strcat(filePath,rgbFile));
depthFile = strcat('depth_',num2str(depthImgNo),'.png');
depthImg = imread(strcat(filePath,depthFile));
rgbImg = double(rgbImg);
depthImg = double(depthImg);
bwImg =  imreadbw(strcat(filePath,rgbFile));

end

