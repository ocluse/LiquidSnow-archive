// Phytensor.QuantumCrypt.cpp : Defines the entry point for the application.
//
#include <stdio.h>
#include <stdlib.h>
#ifndef string
#define string char*
#endif // !string

int main()
{
	printf("Where is my mother\n");
	string str=malloc(sizeof(char)*20);
	int x = 0;
	int y = 0;
	scanf("%s",str);
	scanf("%d", y);

	int z = x + y;
	printf(z);
	//printf(str);
	free(str);
	return 0;
}