function [pWorld] = pixelToWorld(x,y,z,pose,depthK,rgbK,depthT,rgbT,translateRGBToVicon)
%PIXELTOWORLD Summary of this function goes here
%   Detailed explanation goes here

transformRGBToVicon = [eye(3) translateRGBToVicon; 0 0 0 1];

nPoints = length(x);
pWorld = zeros(3,nPoints);

for i = 1:nPoints
    xDepth = [x(i)*z(i);
              y(i)*z(i);
              z(i)];
    XRGB = depthK^-1*xDepth - depthT;
    XViconH = transformRGBToVicon*[XRGB; 1];
    pWorld(:,i) = RelativeToAbsolutePosition(pose,XViconH(1:3));
end


end

