#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int main(int argc, char** argv){
    
	FILE *pipe;
    char *buf = (char*)calloc(1,1024);
    memset(buf, 0 , 1024);

    if(!buf)
        return 1;

    pipe = popen("/home_local/cvr/Desktop/rfcx-espol-server/RfcxServer/WebApplication/icecast/playlist/playlist", "r");

    if(!pipe) {
        printf("Couldn't open pipe to program ");
        return 1;
    }

    printf("%d\n", (int)pipe);


    if(fgets(buf, 1024, pipe) == NULL) {
        printf("%d", ferror(pipe));
        printf("Couldn't read filename from pipe to program ");
        free(buf);
        pclose(pipe);
        return 1;
    }

    printf("%s\n", buf);

	pclose(pipe);
}
