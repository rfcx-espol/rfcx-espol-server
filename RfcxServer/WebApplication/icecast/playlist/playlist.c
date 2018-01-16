#include <stdio.h>
#include <stdlib.h>

int main(int argc, char** argv){
    
	FILE * f = fopen("playlist.txt", "r");

	if(f == NULL){
		printf("ERROR\n");
	}
	
	char path[200];
	fscanf(f, "%s", path);

    fprintf(stdout, "%s", path);

    return 0;
}

