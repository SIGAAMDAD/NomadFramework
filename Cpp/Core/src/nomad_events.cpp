#include <IGameEvent.h>
#include <GameEvent.h>
#include <nomad_core.h>
#include <EASTL/unordered_map.h>

using namespace Nomad::Core;

static eastl::unordered_map<int, IGameEvent *> s_eventCache;

void *nomad_event_get( int hashCode, const char *pszNameSpace, const char *pszName )
{
	IGameEvent *event;

	eastl::unordered_map<int, IGameEvent *>::iterator it;
	if ( ( it = s_eventCache.find( hashCode ) ) != s_eventCache.end() ) {
		return it.get_node()->mValue.second;
	}

	event = new GameEvent(  );
	s_eventCache.try_emplace( hashCode, event );

	return event;
}

void nomad_event_publish( void *event, void *data )
{

}