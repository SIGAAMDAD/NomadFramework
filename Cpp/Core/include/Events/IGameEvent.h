#ifndef __IGAME_EVENT_H__
#define __IGAME_EVENT_H__

#include "nomad_platform.h"
#include "nomad_compiler.h"

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

#include "nomad_core.h"

namespace Nomad::Core
{
	class IGameEvent
	{
	public:
		virtual ~IGameEvent() = 0;

		virtual void Publish( const void *pData ) const = 0;
		virtual void Subscribe( void *pSuscriber, EventCallback callback ) = 0;

		virtual const char *GetName( void ) const = 0;
	};
};

#endif