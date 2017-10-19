function generatePolyscopeScript(poses,fileName)
%GENERATEPOLYSCOPESCRIPT generates polyscope script with list of waypoints

%% 1. open file
if ispc
    sep = '\';
elseif isunix || ismac
    sep = '/';
end
file1 = fopen(strcat(pwd,sep,'polyscope',sep,fileName,'.script'),'w');
file2 = fopen(strcat(pwd,sep,'polyscope',sep,fileName,'_noStops.script'),'w');

%% 2. starting script stuff
fprintf(file1,'while (True):\n');

%% 3. waypoints & wait times
for i = 1:size(poses,2)
    moveStr = strcat('movej(p',...
                 strrep(mat2str(poses(:,i)'),' ',','),...
                 ',a=1',...
                 ',v=1)');
    sleepStr = 'sleep(2.0)';
    formatSpec1 = '\t%s\n\t%s\n';
    formatSpec2 = '\t%s\n';
    fprintf(file1,formatSpec1,moveStr,sleepStr);
    fprintf(file2,formatSpec2,moveStr);
end

%% 4. ending script stuff 
fprintf(file1,'\thalt');
fprintf(file2,'\thalt');
fprintf(file1,'\nend');
if ispc
    fprintf(file2,'\nend');
end
%% 5. close files
fclose(file1);
fclose(file2);

end

