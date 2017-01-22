// RunAsAdmin.cpp : main project file.

#include "stdafx.h"

using namespace System;
using namespace AdmPwd::PDSUtils;
using namespace AdmPwd::Types;

String^ GetBuiltInAdminName();

void Usage();

int main(array<System::String ^> ^args)
{
	String^ adminAccountName;
	String^ pathToExecutable;
	for each (String^ arg in args)
	{
		if (arg->StartsWith(L"/user:", StringComparison::CurrentCultureIgnoreCase))
		{
			adminAccountName = arg->Substring(6);
			continue;
		}
		if (arg->StartsWith(L"/path:", StringComparison::CurrentCultureIgnoreCase))
		{
			pathToExecutable = arg->Substring(6);
			continue;
		}
	}
	try
	{
		//show usage when path not specified
		if (pathToExecutable == nullptr)
		{
			Usage();
			return 0;
		}

		//if account not specified, use built-in admnin
		if (adminAccountName == nullptr)
			adminAccountName = GetBuiltInAdminName();

		//get local computer name
		String^ computerName = System::Environment::GetEnvironmentVariable("COMPUTERNAME");

		//retrieve password for local computer
		PasswordInfo^ pi = PdsWrapper::GetPassword(String::Empty, computerName, false, false);
		//run desired process
		pin_ptr<const wchar_t> _adminAccount = PtrToStringChars(adminAccountName);
		pin_ptr<const wchar_t> _pwd = PtrToStringChars(pi->Password);
		pin_ptr<const wchar_t> _path = PtrToStringChars(pathToExecutable);
		STARTUPINFO startInfo;
		PROCESS_INFORMATION processInfo;

		ZeroMemory(&startInfo, sizeof(startInfo));
		startInfo.cb = sizeof(startInfo);

		ZeroMemory(&processInfo, sizeof(processInfo));

		if (!CreateProcessWithLogonW(_adminAccount, L".", _pwd, LOGON_WITH_PROFILE, nullptr, (LPWSTR)_path, CREATE_NEW_PROCESS_GROUP | ABOVE_NORMAL_PRIORITY_CLASS, nullptr, nullptr, &startInfo, &processInfo))
			throw gcnew System::ComponentModel::Win32Exception(GetLastError(), L"Failed to create process as admin");
	}
	catch (PDSException^ ex)
	{
		Console::WriteLine("ERROR: " + ex->Message);
		return 1;
	}

	catch (System::ComponentModel::Win32Exception^ ex)
	{
		Console::WriteLine("ERROR: " + ex->ErrorCode.ToString() + ": " + ex->Message);
		return 1;
	}
	catch (Exception^ ex)
	{
		Console::WriteLine("ERROR: " + ex->Message);
		return 1;
	}
	return 0;
}

String^ GetBuiltInAdminName()
{
	PSID pSID = NULL;
	BOOL bResult = false;
	TCHAR *acctName = nullptr;
	TCHAR *RefDomain = nullptr;

	try
	{
		DWORD dwNameSize = 0, dwRefDomainSize = 0;
		SID_NAME_USE snu;

		//find out size of necessary buffers
		bResult = ConvertStringSidToSid(L"LA", &pSID);
		bResult = LookupAccountSid(NULL, pSID, NULL, &dwNameSize, NULL, &dwRefDomainSize, &snu);
		if (!bResult)
		{
			DWORD dwErr = GetLastError();
			if (dwErr != ERROR_INSUFFICIENT_BUFFER)
			{
				throw gcnew System::ComponentModel::Win32Exception(dwErr, L"Failed to find built-in admin account");
			}
		}

		//allocate buffers
		acctName = new(std::nothrow) TCHAR[dwNameSize];
		RefDomain = new(std::nothrow) TCHAR[dwRefDomainSize];
		if (acctName == NULL || RefDomain == NULL)
		{
			throw gcnew System::ComponentModel::Win32Exception(ERROR_NOT_ENOUGH_MEMORY, L"Failed to find built-in admin account");
		}

		//find out built-in admin account name
		bResult = LookupAccountSid(NULL, pSID, acctName, &dwNameSize, RefDomain, &dwRefDomainSize, &snu);
		if (!bResult)
		{
			throw gcnew System::ComponentModel::Win32Exception(GetLastError(), L"Failed to find built-in admin account");
		}
		return gcnew String(acctName);
	}
	finally
	{

		if (pSID)
			LocalFree(pSID);
		if (RefDomain != nullptr)
			delete[] RefDomain;
		if (acctName != nullptr)
			delete[] acctName;
	}
}

void Usage()
{
	Console::WriteLine("ERROR: Path argument must be specified\n");
	Console::WriteLine("Usage:");
	Console::WriteLine("RunAsAdmin.exe /path:<path to executable to run> [/user:<name of local admin account>]");
}
