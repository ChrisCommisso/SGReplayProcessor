#include <iostream>
#include <Windows.h>
#include <Psapi.h>

using namespace std;

DWORD GetBaseAddress(const HANDLE hProcess) {
	if (hProcess == NULL)
		return NULL; // No access to the process

	HMODULE lphModule[1024]; // Array that receives the list of module handles
	DWORD lpcbNeeded(NULL); // Output of EnumProcessModules, giving the number of bytes requires to store all modules handles in the lphModule array

	if (!EnumProcessModules(hProcess, lphModule, sizeof(lphModule), &lpcbNeeded))
		return NULL; // Impossible to read modules

	TCHAR szModName[MAX_PATH];
	if (!GetModuleFileNameEx(hProcess, lphModule[0], szModName, sizeof(szModName) / sizeof(TCHAR)))
		return NULL; // Impossible to get module info

	return (DWORD)lphModule[0]; // Module 0 is apparently always the EXE itself, returning its address
}

int main()
{
	int readP2Win = 0;
	int readP1Win = 0;
	HWND hwnd = FindWindowA(NULL, "Skullgirls Encore");
	if (hwnd == NULL)
	{
		cout << "Cannot Find Skullgirls." << endl;
		Sleep(3000);
		exit(-1);
	}
	else
	{
		DWORD procID;
		GetWindowThreadProcessId(hwnd, &procID);
		HANDLE handle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, procID);

		DWORD AddressOfFirstPointer, ValueAtFirstPointer, AddressOfP1Win, AddressOfP2Win, ValueOfP2Win, ValueOfP1Win;
		DWORD P1WinOffset = 0x434;
		DWORD P2WinOffset = 0x43C;
		DWORD BaseAddress = GetBaseAddress(handle);
		DWORD BaseAddressOffset = 0x00841984;
		
		//Uncomment the print statements for de-bugging reasons!

		//cout << "The Base Address of SkullGirls is: " << std::hex << BaseAddress << endl;

		AddressOfFirstPointer = BaseAddress + BaseAddressOffset;

		//cout << "The Address of The Static Pointer To UI CLass Object is: " << std::hex << AddressOfFirstPointer << endl;

		ReadProcessMemory(handle, (DWORD*)AddressOfFirstPointer, &ValueAtFirstPointer, sizeof(int), 0);

		//cout << "The Value of The Static Pointer To UI CLass Object is: " << std::hex << ValueAtFirstPointer << endl;

		AddressOfP1Win = ValueAtFirstPointer + P1WinOffset;

		//cout << "The Address of The Player 1 Win Bool is: " << std::hex << AddressOfP1Win << endl;

		ReadProcessMemory(handle, (DWORD*)AddressOfP1Win, &ValueOfP1Win, sizeof(int), 0);

		//cout << "The Value of The Player 1 Win Bool is: " << std::hex << ValueOfP1Win << endl;

		AddressOfP2Win = ValueAtFirstPointer + P2WinOffset;

		//cout << "The Address of The Player 2 Win Bool is: " << std::hex << AddressOfP2Win << endl;

		ReadProcessMemory(handle, (DWORD*)AddressOfP2Win, &ValueOfP2Win, sizeof(int), 0);

		//cout << "The Value of The Player 2 Win Bool is: " << std::hex << ValueOfP2Win << endl;

		if (procID == NULL)
		{
			cout << "Cannot Find Skullgirls Proccess ID" << endl;
			Sleep(3000);
			exit(-1);
		}
		else
		{
			cout << "Press Num Pad 0 to Exit..." << endl;
			cout << "Starting Python Server..." << endl;
			
			int seconds;
			while (true) 
			{
				seconds = 0;
				ReadProcessMemory(handle, (PBYTE*)AddressOfP1Win, &readP1Win, sizeof(int), 0);
				ReadProcessMemory(handle, (PBYTE*)AddressOfP2Win, &readP2Win, sizeof(int), 0);
				if (!readP1Win && !readP2Win) 
				{
					while (true) 
					{
						Sleep(1000);
						seconds++;
						ReadProcessMemory(handle, (PBYTE*)AddressOfP1Win, &readP1Win, sizeof(int), 0);
						ReadProcessMemory(handle, (PBYTE*)AddressOfP2Win, &readP2Win, sizeof(int), 0);
						if (seconds > 240)
						{
							cout << "No Winner Could Be Found...\n" << endl;
							system("curl --location --request GET localhost:8080 --form winner='None'");
							break;

						}
						if (readP1Win)
						{
							cout << "Player 1 Wins!" << endl;
							system("curl --location --request GET localhost:8080 --form winner='Player1'");
							
							break;
						}
						if (readP2Win)
						{
							cout << "Player 2 Wins!" << endl;
							system("curl --location --request GET localhost:8080 --form winner='Player2'");
							
							break;

						}
						if (!(readP2Win) && !(readP1Win))
						{
							cout << "Game In Progress..." << endl;
							
						}
						if (GetAsyncKeyState(VK_NUMPAD0)) { // Exit
							return 0;
						}
					}
				}
			}
			exit(-1);
			}
		}
	}
