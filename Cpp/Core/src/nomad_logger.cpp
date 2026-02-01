#include <ILoggerService.h>
#include <nomad_core.h>

using namespace Nomad::Core;

static ILoggerService *s_pLogger;

void nomad_logger_init( void )
{
}

void nomad_logger_flush( void )
{
}

void nomad_logger_print( int eLevel, const char* pszMessage )
{
}