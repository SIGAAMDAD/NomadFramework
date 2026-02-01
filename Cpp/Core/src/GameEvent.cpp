#include <GameEvent.h>

using namespace Nomad::Core;

GameEvent::GameEvent( int hashCode, const char *pszName )
	: m_szName( pszName ), m_nHashCode( hashCode )
{
	m_EventList.reserve( 72 );
}

GameEvent::~GameEvent()
{
	m_EventList.clear();
}

void GameEvent::Subscribe( void *pSubscriber, EventCallback callback )
{
	m_EventList.emplace_back( pSubscriber, callback );
}

void GameEvent::Publish( const void *pData ) const
{
	for ( int i = 0; i < m_EventList.size(); ++i ) {
		m_EventList[ i ]( pData );
	}
}