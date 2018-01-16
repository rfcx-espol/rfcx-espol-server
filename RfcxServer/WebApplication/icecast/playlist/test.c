#include <stdio.h>
#include <stdlib.h>

int main(int argc, char** argv){
    
	FILE *pipe;
    char *buf = (char*)calloc(1,1024);

    if(!buf)
        return 1;

    pipe = popen("/home/javier/Documents/pasantias/rfcx-espol-server/RfcxServer/WebApplication/icecast/playlist/playlist", "r");

    if(!pipe) {
        printf("Couldn't open pipe to program ");
        return 1;
    }

    if(fgets(buf, 1024, pipe) == NULL) {
        printf("Couldn't read filename from pipe to program ");
        free(buf);
        pclose(pipe);
        return 1;
    }

    printf("%s\n", buf);

	pclose(pipe);
}
