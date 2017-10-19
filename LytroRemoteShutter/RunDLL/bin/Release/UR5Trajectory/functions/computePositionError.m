function [error] = computePositionError(x1,x2,y1,y2,z1,z2,pose1,pose2,depthK,rgbK,depthT,rgbT,translateRGBToVicon)
%COMPUTEPOSITIONERROR Summary of this function goes here
%   Detailed explanation goes here

points1 = pixelToWorld(x1,y1,z1,pose1,depthK,rgbK,depthT,rgbT,translateRGBToVicon);
points2 = pixelToWorld(x2,y2,z2,pose2,depthK,rgbK,depthT,rgbT,translateRGBToVicon);
error = norm(points1-points2);

end

