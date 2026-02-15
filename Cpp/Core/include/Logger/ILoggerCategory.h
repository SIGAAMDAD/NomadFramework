#ifndef __ILOGGER_CATEGORY_H__
#define __ILOGGER_CATEGORY_H__

#include "nomad_platform.h"

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

#include <EASTL/string.h>
#include "nomad_core.h"

namespace Nomad::Core
{
	class ILoggerCategory
	{
	public:
		ILoggerCategory( const char *pszName, int eLevel );
		virtual ~ILoggerCategory();
	protected:
		const char* m_pszName;
	};
};

#endif