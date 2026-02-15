#include <GameEvent.h>
#include <LoggerService.h>
#include <LoggerSink.h>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/godot.hpp>
#include <godot_cpp/core/defs.hpp>
#include <gdextension_interface.h>

using namespace Nomad::Core;
using namespace godot;

class GodotGameEvent : public GameEvent
{
};

void initialize_nomadframework( ModuleInitializationLevel pLevel )
{
	if ( pLevel != MODULE_INITIALIZATION_LEVEL_SCENE ) 
	{
		return;
	}

	ClassDB::register_class<GameEvent>();
	ClassDB::register_class<LoggerService>();
}

void deinitialize_nomadframework( ModuleInitializationLevel pLevel )
{
}

extern "C"
{

GDExtensionBool GDE_EXPORT nomadframework_init(
	GDExtensionInterfaceGetProcAddress pGetProcAddress,
	GDExtensionClassLibraryPtr pLibrary,
	GDExtensionInitialization *pInitialization
)
{
	GDExtensionBinding::InitObject initObj( pGetProcAddress, pLibrary, pInitialization );
	initObj.register_initializer( initialize_nomadframework );
	initObj.register_terminator( deinitialize_nomadframework );
	initObj.set_minimum_library_initialization_level( MODULE_INITIALIZATION_LEVEL_SCENE );
	return initObj.init();
}

}