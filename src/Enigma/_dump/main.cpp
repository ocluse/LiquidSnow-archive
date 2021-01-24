#include "aero.h"
using namespace std;
//void fillGrid();
//DEFINITIONS OF FUNCTIONS
void ceaser_hack();
void vinegere_hack();
void load_dictionary();
void deferred();
string remove_space(string input);
bool dict_check(string input);
bool logic_check(string input);
string upper(string val);
string vinegere_decypher(string input, string key);
string vinegere_cypher(string input, string key);
string playfair_cypher(string input, string key);
string playfair_decypher(string input, string key, char omit);
int letter_value(char letter);
bool vin_detective(char fst, char sec, char thd);
//GLOBAL VARIABLES
string dictionary[466457];


string key;
char letters[] = { 'A', 'B','C','D','E','F','G','H','I','J','K','L','M',
'N','O','P','Q','R','S','T','U','V','W','X','Y','Z' };
int main() 
{
	load_dictionary();
	
	deferred();
	vinegere_hack();
	int x;
	cin >> x;
}

void deferred() 
{
	cout << "Give us a sentence: ";
	string sentence;
	getline(cin, sentence);
	cout << "Give us a key: ";
	string key;
	getline(cin, key);
	string veg = vinegere_cypher(sentence, key);
	cout << "VIGENERE: " << veg<<endl;
	cout << "Attempt to perform a Hack: " << endl;
	vinegere_hack();
}
void ceaser_hack() 
{
	string ciphtext;
	string posvals[25];
	cout << "Enter text to be decrypted: ";
	getline(std::cin,ciphtext);	
	//Next we work out the length of our plaintext:
	int len=0;
	while (ciphtext[len] != NULL) {
		len++;
	}
	//then we begin to decypher:
	for (int key = 0; key < 26; key++)
	{
		string posval;
		for (int i = 0; i < len; i++) 
		{
			char fig;
			if (ciphtext[i] != ' ') 
			{
				int val = 0;
				while (letters[val] != ciphtext[i]) {
					val++;
				}
				val -= key;
				if (val < 0)val += 26;
				fig = letters[val];
			}
			else 
			{
				fig = ' ';
			}
			posval += fig;		
		}
		cout << "Key #" << key << ": " << posval << endl;
	}
	
	main();
}
void load_dictionary() 
{
	cout << "Started loading Dictionary"<<endl;
	int loader = 0;
	string word;
	ifstream file;
	file.open("words.txt");
	while (getline(file, word))
	{
		word = upper(word);
		word = remove_space(word);
		dictionary[loader] = word;
		loader++;
	
	}
	cout << "Finished loading dictionary with " << loader << "Entries" << endl;
}
bool logic_check(string test)
{
	char* check;
	bool result;
	int len = test.length;
	//Check the first character whether it is an A or I:
	if (test[0] == 'A')
	{
		if (test[1] == 'N') {
			//Pass the test from this point, there is the letter AN in english
			len -= 2;
			check = new (nothrow) char[len];

			//Construct the check:
			for (int i = 2; i < test.length - 2; i++) {
				check[i] = test[i];
			}

			//Perform the dict check
			if (dict_check(check)) {
				return true;
			}
		}

		//Construct the other check
		check = new (nothrow) char[test.length - 1];

		for (int p = 1; p < test.length - 1; p++) 
		{
			check[p] = test[p];
		}
		if (dict_check(check)) {
			return true;
		}
	}
	if (test[0] == 'I') {
		check = new (nothrow) char[test.length - 1];
		//Redo as in A
		for (int p = 1; p < test.length - 1; p++) {
			check[p] = test[p];
		}
		if (dict_check(check))return true;
	}

	if (dict_check(test))return true;
	return false;
}
bool dict_check(string input) {
	//check each and every letter, as long as the letter is at least three letters:
	for (int i = 0; i < 466457; i++)
	{
		bool sally = true;
		if (dictionary[i].length > 1  && input.length>=dictionary[i].length) {
			//construct a letter array:
			for (int x = 0; x < input.length;i++) {
				if (input[x] != dictionary[i][x]) {
					sally = false;
					break;
				}
			}
		}
		if (sally)
		{
			return true;
			break;
		}
	}
	return false;
}
void vinegere_hack() 
{
	string ciphertext;
	cout << "Enter the cypher text: ";
	getline(cin, ciphertext);
	ciphertext = upper(ciphertext);
	ciphertext = remove_space(ciphertext);
	int posssible=0;
	for(int check=0;check<10000;check++)
	{
		string test_key = dictionary[check];
		string resltant = vinegere_decypher(ciphertext, test_key);
		if (logic_check(resltant))
		{
			cout << "Using key:" << test_key << ">>>>" << resltant<<endl;
			posssible++;
		}
	}
	cout << "Finished Hack with" << posssible << "Possibilities"<<endl;
	vinegere_hack();
}
bool vin_detective(char fst,char sec, char thd) {
	bool result = false;
	if (fst == 'B') 
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'L' || sec == 'O' || sec == 'U' || sec == 'Y')
		{
			result = true;
			if (thd == 'A' || thd == 'X' || thd == 'I' || thd == 'L' || thd == 'O' ||   thd == 'Y')
			{
				result = true;

			}
		}
	}
	if (fst == 'C')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'L' || sec == 'O' || sec == 'R' || sec == 'U' || sec == 'Y')
		{
			result = true;
		}
	}
	if (fst == 'D')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'O' || sec == 'R' || sec == 'U')
		{
			result = true;
		}
	}
	if (fst == 'E')
	{
		if (!(sec == 'H' || sec == 'O' || sec == 'W' || sec == 'E'))
		{
			result = true;
		}
	}
	if (fst == 'F')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'L' || sec == 'O' || sec == 'R' || sec == 'U')
		{
			result = true;
		}
	}
	if (fst == 'G')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'L' || sec == 'O' || sec == 'R' || sec == 'U' || sec == 'Y')
		{
			result = true;
		}
	}
	if (fst == 'H')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'O' || sec == 'U' || sec == 'Y')
		{
			result = true;
		}
	}
	if (fst == 'I')
	{
		if (!(sec == 'X'))
		{
			result = true;
		}
	}
	if (fst == 'J')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'O' || sec == 'U')
		{
			result = true;
		}
	}
	if (fst == 'K')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'N' || sec == 'O')
		{
			result = true;
		}
	}
	if (fst == 'L')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'U' || sec == 'Y')
		{
			result = true;
		}
	}
	if (fst == 'M')
	{
	if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'O' || sec == 'Y')
	{
		result = true;
	}
	}
	if (fst == 'N')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'O' || sec == 'U')
		{
			result = true;
		}
	}
	if (fst == 'O')
	{
		if (!(sec == 'Q' || sec == 'O'))
		{
			result = true;
		}
	}
	if (fst == 'P')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'L' || sec == 'O' || sec == 'N' || sec == 'R' || sec == 'U')
		{
			result = true;
		}
	}
	if (fst == 'Q')
	{
		if (sec == 'U')
		{
			result = true;
		}
	}
	if (fst == 'R')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'O' || sec == 'U' || sec == 'H')
		{
			result = true;
		}
	}
	if (fst == 'S')
	{
		if (sec == 'A' || sec == 'C' || sec == 'E' || sec == 'H' || sec == 'I' || sec == 'L' || sec == 'N' || sec == 'O' || sec == 'P' || sec == 'T' || sec == 'U' || sec == 'Y')
		{
			result = true;
		}
	}
	if (fst == 'T')
	{
		if (sec == 'A' || sec == 'E' || sec == 'H' || sec == 'I' || sec == 'O' || sec == 'R' || sec == 'U')
		{
			result = true;
		}
	}
	if (fst == 'U')
	{
		if (sec == 'G' || sec == 'N')
		{
			result = true;
		}
	}
	if (fst == 'V')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'O' || sec == 'U')
		{
			result = true;
		}
	}
	if (fst == 'W')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'O')
		{
			result = true;
		}
	}
	if (fst == 'X')
	{
		if (sec == 'A')
		{
			result = true;
		}
	}
	if (fst == 'Y')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'O' || sec == 'U')
		{
			result = true;
		}
	}
	if (fst == 'Z')
	{
		if (sec == 'A' || sec == 'E' || sec == 'I' || sec == 'O')
		{
			result = true;
		}
	}
	return result;
}
string upper(string val) 
{
	string output=val;
	for (int i = 0; i < val.size(); i++)
	{
		output.at(i) = toupper(val.at(i));
		
	}
	return output;
}
string vinegere_cypher(string input,string key) 
{
	int key_indexer = 0;
	string plaintext=input;
	plaintext = remove_space(plaintext);
	plaintext = upper(plaintext);
	string ciphertext = plaintext;
	for (int i = 0; i <= ciphertext.size(); i++) 
	{
		if (key_indexer == key.size()) {
			key_indexer = 0;
		}
		int ciph = letter_value(key[key_indexer]) + letter_value(plaintext[i]);		
		ciph++;
		if (ciph > 25 )ciph -= 26;
		ciphertext[i] = letters[ciph];
		key_indexer++;
	}
	return ciphertext;
}
string vinegere_decypher(string input, string key)
{
	int key_indexer = 0;
	string ciphertext = input;
	ciphertext = remove_space(ciphertext);
	ciphertext = upper(ciphertext);
	string plaintext=ciphertext;
	for (int i = 0; i <= ciphertext.size(); i++)
	{
		if (key_indexer == key.size()) {
			key_indexer = 0;
		}
		int deciph =letter_value(ciphertext[i])-letter_value(key[key_indexer]);
		deciph--;
		if (deciph < 0)deciph += 26;
		plaintext[i] = letters[deciph];
		key_indexer++;
	}
	return plaintext;
}
string playfair_cypher(string input, string key) 
{
	//first create and fill key grid;
	char key_grid[5][5];
	string skey=upper(key);
	//First we shorten the key:
	for (int i = 0; i < key.size(); i++)
	{
		bool exists = false;
		for (int check = 0; check < i; check++)
		{
			if (skey[i] == skey[check])  exists = true;
		}
		if (exists) 
		{
			skey[i] = ' ';
		}	
	}
	skey = remove_space(skey);
	//We create our shortened alphabet:
	string salpha="ABCDEFGHIJKLMNOPRQSTUVWXYZ";
	for (int i = 0; i < salpha.size(); i++)
	{
		bool exists = false;
		for (int check = 0; check < skey.size(); check++)
		{			
			if (salpha[i] == skey[check])exists = true;
		}
		if (exists || salpha[i]=='Q') {
			salpha[i] = ' ';
		}
	}
	salpha=remove_space(salpha);
	//Then we fit it all up in the key grid:
	skey += salpha;
	int col = 0;
	int row = 0;
	for (int i = 0; i < 25; i++) 
	{
		if (col > 4) {
			row++;
			col = 0;
		}
		key_grid[row][col] = skey[i];
		col++;
	}
	//Now we can begin Cyphering:::::
	//Fist we prepare our plaintext:
	string plaintext = upper(input);
	plaintext = remove_space(plaintext);
	
	if (plaintext.size() % 2 != 0)plaintext += 'Z'; //To maje it even
	string ciphertext = plaintext;
	//We begin to do the Cyphering, for real this time:
	for (int i = 0; i < (plaintext.size() / 2); i++) 
	{
		//We locate our diagraphs in the grid first:
		int cell1[2];
		int cell2[2];
		//The first cell:
		col = 0;
		row = 0;
		for (int check = 0; check < 25; check++) 
		{
			if (col > 4) {
				row++;
				col = 0;
			}
			if (plaintext[i * 2] == key_grid[row][col]) 
			{
				cell1[0] = row;
				cell1[1] = col;
			}
			if (plaintext[(i * 2) + 1] == key_grid[row][col]) 
			{
				cell2[0] = row;
				cell2[1] = col;
			}
			col++;
		}
		//Now we move to the next step, filling in the cyphertext:
		//STARTING: If they occur in different cols and rows:
		if (cell1[0] != cell2[0] && cell1[1] != cell2[1])
		{
			int temp = cell1[1];
			cell1[1] = cell2[1];
			cell2[1] = temp;
			
		}
		//STARTING: If they fall in same cols diff rows:
		else if (cell1[0] != cell2[0] && cell1[1] == cell2[1]) 
		{
			cell1[0]++;
			cell2[0]++;
		}
		//STARTING: If they fall in the same row:
		else if (cell1[0] == cell2[0] && cell1[1] != cell2[1]) 
		{
			cell1[1]++;
			cell2[1]++;
		}
		//STARTING: They fall in the same cell:
		else 
		{
			cell1[0]++;
			cell2[0]++;
		}
		if (cell1[0] > 4)cell1[0] = 0;
		if (cell2[0] > 4)cell2[0] = 0;
		if (cell1[1] > 4)cell1[1] = 0;
		if (cell2[1] > 4)cell2[1] = 0;
		//Now we finally add the value to ciphertext, finally!!!! :)
		ciphertext[(i * 2)] = key_grid[cell1[0]][cell1[1]];
		ciphertext[(i * 2) + 1] = key_grid[cell2[0]][cell2[1]];
	}
	
	
	
	return ciphertext;
	
}
string playfair_decypher(string input, string key, char omit)
{
	//first create and fill key grid;
	char key_grid[5][5];
	string skey = upper(key);
	//First we shorten the key:
	for (int i = 0; i < key.size(); i++)
	{
		bool exists = false;
		for (int check = 0; check < i; check++)
		{
			if (skey[i] == skey[check])  exists = true;
		}
		if (exists)
		{
			skey[i] = ' ';
		}
	}
	skey = remove_space(skey);
	//We create our shortened alphabet:
	string salpha = "ABCDEFGHIJKLMNOPRQSTUVWXYZ";
	for (int i = 0; i < salpha.size(); i++)
	{
		bool exists = false;
		for (int check = 0; check < skey.size(); check++)
		{
			if (salpha[i] == skey[check])exists = true;
		}
		if (exists || salpha[i] == omit) {
			salpha[i] = ' ';
		}
	}
	salpha = remove_space(salpha);
	//Then we fit it all up in the key grid:
	skey += salpha;
	int col = 0;
	int row = 0;
	for (int i = 0; i < 25; i++)
	{
		if (col > 4) {
			row++;
			col = 0;
		}
		key_grid[row][col] = skey[i];
		col++;
	}
	//Now we can begin Cyphering:::::
	//Fist we prepare our ciphertext:
	string ciphertext = upper(input);
	ciphertext = remove_space(ciphertext);

	if (ciphertext.size() % 2 != 0)ciphertext += 'Z'; //To maje it even
	string plaintext = ciphertext;
	//We begin to do the Cyphering, for real this time:
	for (int i = 0; i < (ciphertext.size() / 2); i++)
	{
		//We locate our diagraphs in the grid first:
		int cell1[2];
		int cell2[2];
		//The first cell:
		col = 0;
		row = 0;
		for (int check = 0; check < 25; check++)
		{
			if (col > 4) {
				row++;
				col = 0;
			}
			if (ciphertext[i * 2] == key_grid[row][col])
			{
				cell1[0] = row;
				cell1[1] = col;
			}
			if (ciphertext[(i * 2) + 1] == key_grid[row][col])
			{
				cell2[0] = row;
				cell2[1] = col;
			}
			col++;
		}
		//Now we move to the next step, filling in the cyphertext:
		//STARTING: If they occur in different cols and rows:
		if (cell1[0] != cell2[0] && cell1[1] != cell2[1])
		{
			int temp = cell1[1];
			cell1[1] = cell2[1];
			cell2[1] = temp;

		}
		//STARTING: If they fall in same cols diff rows:
		else if (cell1[0] != cell2[0] && cell1[1] == cell2[1])
		{
			cell1[0]--;
			cell2[0]--;
		}
		//STARTING: If they fall in the same row:
		else if (cell1[0] == cell2[0] && cell1[1] != cell2[1])
		{
			cell1[1]--;
			cell2[1]--;
		}
		//STARTING: They fall in the same cell:
		else
		{
			cell1[0]--;
			cell2[0]--;
		}
		if (cell1[0] < 0)cell1[0] = 4;
		if (cell2[0] < 0)cell2[0] = 4;
		if (cell1[1] < 0)cell1[1] = 4;
		if (cell2[1] < 0)cell2[1] = 4;
		//Now we finally add the value to plaintext, finally!!!! :)
		plaintext[(i * 2)] = key_grid[cell1[0]][cell1[1]];
		plaintext[(i * 2) + 1] = key_grid[cell2[0]][cell2[1]];
	}



	return plaintext;

}
string remove_space(string input) 
{
	string result=input;
	int length = input.size();
	for (int i = 0; i <= result.size(); i++) {
		if (result[i] == ' ') 
		{
			//first we shift the letters
			for (int rep = i; rep < result.size(); rep++) 
			{
				result[rep] = result[rep + 1];
			}
			//then next we remove the white space created at the end:
			length--;
			
		}
		result.resize(length);
	}
	//Then we check if there are any more spaces
	for (int i = 0; i <= result.size(); i++) {
		if (result[i] == ' ')
		{
			result= remove_space(result);
		}
	}
	return result;
}
int letter_value(char letter) 
{
	int val=0;
	while (letter != letters[val]) {
		val++;
	}
	return val;
}