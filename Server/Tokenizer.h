//ΩÃ±€≈Ê
//#ifndef _MUTITOUCH_H5
//#define _MUTITOUCH_H5
#pragma once
#include <string>
#include <vector>

using namespace std;

class Tokenizer{
public:
	static void Tokenize(const string& str,
		vector<string>& tokens,
		const string& delimiters = "&");
};

//#endif