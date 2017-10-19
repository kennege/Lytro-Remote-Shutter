function [error] = computeDepthToRGBError(xRGB,yRGB,xDepth,yDepth,zDepth,rgbK,rgbT,depthK,depthT)
%COMPUTEDEPTHTORGBERROR Summary of this function goes here
%   Detailed explanation goes here

nPoints = length(xRGB);
error = 0;
for i = 1:nPoints
    pWorldDepth = [xDepth(i)*zDepth(i);
                   yDepth(i)*zDepth(i)
                   zDepth(i)];
    pWorldRGB   = rgbK*(depthK^-1*pWorldDepth - depthT) + rgbT;
    xRGBPred    = pWorldRGB(1)/pWorldRGB(3);
    yRGBPred    = pWorldRGB(2)/pWorldRGB(3);
    
    error = error + sqrt((xRGBPred - xRGB(i))^2 + (yRGBPred - yRGB(i))^2);
    
end

end

