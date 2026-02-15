#ifndef __GAME_EVENT_H__
#define __GAME_EVENT_H__

#include "nomad_platform.h"
#include "nomad_compiler.h"

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

#include "IGameEvent.h"
#include <EASTL/vector.h>
#include <EASTL/fixed_string.h>

namespace Nomad::Core
{
	class GameEvent : public IGameEvent
	{
	public:
		GameEvent( int hashCode, const char *pszNameSpace, const char *pszName );
		virtual ~GameEvent() override;

		virtual void Subscribe( void *pSubscriber, EventCallback callback ) override;
		virtual void Publish( const void *pData ) const override;

		virtual const char *GetName( void ) const override;
	protected:
		const eastl::fixed_string<char, 128> m_szName;
		const eastl::fixed_string<char, 72> m_szNameSpace;
		const int m_nHashCode;

		eastl::vector<EventCallback> m_EventList;
	};

	NOMAD_FORCEINLINE const char *GameEvent::GetName( void ) const
	{
		return m_szName.c_str();
	}
};

#endif