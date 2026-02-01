#ifndef __IFILE_SYSTEM_H__
#define __IFILE_SYSTEM_H__

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

namespace Nomad::Core
{
	class IFileSystem
	{
	public:
		virtual void *OpenFile( const char *pszFilePath ) = 0;
	private:
	};
};

#endif