#ifndef __NOMAD_COMPILER_H__
#define __NOMAD_COMPILER_H__

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

#include "nomad_platform.h"

#if defined(__ARMCC_VERSION)
	#define NOMAD_COMPILER_RVCT 1
	#define NOMAD_COMPILER_ARM 1
	#define NOMAD_COMPILER_VERSION __ARMCC_VERSION
	#define NOMAD_COMPILER_NAME "RVCT"
#elif defined(__GNUC__)
	#if defined(__MINGW32__) || defined(__MINGW64__)
		#define NOMAD_COMPILER_NAME "MingW"
	#elif defined(__GNUG__)
		#define NOMAD_COMPILER_NAME "GCC"
	#endif
	#define NOMAD_COMPILER_GCC 1
#elif defined(_MSC_VER)
	#define NOMAD_COMPILER_NAME "MSVC"
	#define NOMAD_COMPILER_MSVC 1
#elif defined(__clang__)
	#define NOMAD_COMPILER_NAME "clang"
#endif

#define NOMAD_XSTRING_HELPER( x ) #x
#define NOMAD_XSTRING( x ) NOMAD_XSTRING_HELPER( x )

#if defined(__cplusplus)
	#if __cplusplus == 1
		#error Please use a modern version of C++
	#endif
	#if __cplusplus >= 199711L
		#define NOMAD_CPP98
	#endif
	#if __cplusplus >= 201103L
		#define NOMAD_CPP11
	#endif
	#if __cplusplus >= 201402L
		#define NOMAD_CPP14
	#endif
	#if __cplusplus >= 201703L
		#define NOMAD_CPP17
	#endif
	#if __cplusplus >= 202002L
		#define NOMAD_CPP20
	#endif
	#if __cplusplus >= 202302L
		#define NOMAD_CPP23
	#endif
#endif

#if !defined(NOMAD_DLL_EXPORT)
	#if NOMAD_PLATFORM_WINDOWS == 1
		#if defined(NOMAD_DLL_COMPILE)
			#define NOMAD_DLL_EXPORT extern "C" __declspec(dllexport)
		#else
			#define NOMAD_DLL_EXPORT extern "C" __declspec(dllimport)
		#endif
	#elif defined(NOMAD_COMPILER_GCC)
		#define NOMAD_DLL_EXPORT extern "C" __attribute__((visibility("default")))
	#endif
#endif

#if !defined(NOMAD_NORETURN)
	#if defined(NOMAD_COMPILER_MSVC)
		#define NOMAD_NORETURN __declspec(noreturn)
	#elif defined(NOMAD_COMPILER_GCC)
		#define NOMAD_NORETURN __attribute__((noreturn))
	#elif defined(NOMAD_CPP11)
		#define NOMAD_NORETURN [[noreturn]]
	#endif
#endif

#if !defined(NOMAD_DEPRECATED)
	#if defined(NOMAD_COMPILER_GCC)
		#if defined(SIRENGINEC_CPP14)
			#define NOMAD_DEPRECATED( reason ) [[deprecated( reason )]]
		#else
			#define NOMAD_DEPRECATED( reason ) __attribute__((deprecated))
		#endif
	#elif defined(NOMAD_CPP14)
		#define NOMAD_DEPRECATED( reason ) [[deprecated( reason )]]
	#else
		#define NOMAD_DEPRECATED( reason )
	#endif
#endif

// Resolve which function signature macro will be used. Note that this only
// is resolved when the (pre)compiler starts, so the syntax highlighting
// could mark the wrong one in your editor!
#if defined(__GNUC__) || (defined(__MWERKS__) && (__MWERKS__ >= 0x3000)) || (defined(__ICC) && (__ICC >= 600)) || defined(__ghs__)
	#define NOMAD_FUNC_NAME __PRETTY_FUNCTION__
#elif defined(__DMC__) && (__DMC__ >= 0x810)
	#define NOMAD_FUNC_NAME __PRETTY_FUNCTION__
#elif (defined(__FUNCSIG__) || (_MSC_VER))
	#define NOMAD_FUNC_NAME __FUNCSIG__
#elif (defined(__INTEL_COMPILER) && (__INTEL_COMPILER >= 600)) || (defined(__IBMCPP__) && (__IBMCPP__ >= 500))
	#define NOMAD_FUNC_NAME __FUNCTION__
#elif defined(__BORLANDC__) && (__BORLANDC__ >= 0x550)
	#define NOMAD_FUNC_NAME __FUNC__
#elif defined(__STDC_VERSION__) && (__STDC_VERSION__ >= 199901)
	#define NOMAD_FUNC_NAME __func__
#elif defined(__cplusplus) && (__cplusplus >= 201103)
	#define NOMAD_FUNC_NAME __func__
#else
	#define NOMAD_FUNC_NAME "NOMAD_FUNC_NAME unknown!"
#endif

#if !defined(NOMAD_ATTRIBUTE)
	#if defined(NOMAD_COMPILER_GCC)
		#define NOMAD_ATTRIBUTE(x) __attribute__((x))
	#else
		#define NOMAD_ATTRIBUTE(x)
	#endif
#endif

#if !defined(NOMAD_FORCEINLINE)
	#if defined(NOMAD_COMPILER_MSVC)
		#define NOMAD_FORCEINLINE __forceinline
	#elif defined(NOMAD_COMPILER_GCC)
		#define NOMAD_FORCEINLINE __attribute__((always_inline)) inline
	#else
		#define SIRENGIEN_FORCEINLINE inline
	#endif
#endif

#if !defined(NOMAD_CONSTEXPR)
	#if defined(NOMAD_COMPILER_CLANG)
		#if __has_feature(__cpp_constexpr) || defined(NOMAD_CPP14)
			#define NOMAD_CONSTEXPR constexpr
		#endif
	#else
		#if defined(NOMAD_CPP14)
			#define NOMAD_CONSTEXPR constexpr
		#else
			#define NOMAD_CONSTEXPR const
		#endif
	#endif
#endif

#if !defined(NOMAD_STATIC_ASSERT)
	#if defined(NOMAD_CPP11)
		#define NOMAD_STATIC_ASSERT( x, str ) static_assert( x, str )
	#else
		#define NOMAD_STATIC_ASSERT( x, str ) if ( !( x ) ) assert( ( 0 ) && str )
	#endif
#endif

#if !defined(NOMAD_DISABLE_ALL_VC_WARNINGS)
	#if defined(NOMAD_COMPILER_MSVC)
		#define NOMAD_DISABLE_ALL_VC_WARNINGS() \
					__pragma( warning( push, 0 ) ) \
					__pragma( warning( disable :  ) )
	#else
		#define NOMAD_DISABLE_ALL_VC_WARNINGS()
	#endif
#endif

#if !defined(NOMAD_RESTORE_ALL_VC_WARNINGS)
	#if defined(NOMAD_COMPILER_MSVC)
		#define NOMAD_RESTORE_ALL_VC_WARNINGS() \
					__pragma( warning( pop ) )
	#else
		#define NOMAD_RESTORE_ALL_VC_WARNINGS()
	#endif
#endif

#if !defined(NOMAD_CACHE_PREFETCH)
	#if defined(NOMAD_COMPILER_GCC)
		#define NOMAD_CACHE_PREFETCH( addr, len ) (__builtin_prefetch( (const void *)( addr ), \
													( ( len ) >> 2 ) & 1, ( len ) & 0x3 ) )
	#else
		#define NOMAD_CACHE_PREFETCH( addr, len ) _mm_prefetch( addr, len )
	#endif
#endif

#if !defined(NOMAD_MAX_OSPATH)
	#if defined(NOMAD_PLATFORM_WINDOWS)
		#define NOMAD_MAX_OSPATH MAX_PATH
	#else
		#define NOMAD_MAX_OSPATH PATH_MAX
	#endif
#endif

#if defined(NOMAD_COMPILER_MSVC) || defined(NOMAD_COMPILER_GCC) || defined(NOMAD_PLATFORM_APPLE)
	#define NOMAD_PRAGMA_ONCE_SUPPORTED 1
#endif

#if !defined(NOMAD_VA_COPY_ENABLED)
	#if ((defined(__GNUC__) && (__GNUC__ >= 3)) || defined(__clang__)) && (!defined(__i386__) || defined(__x86_64__)) && !defined(__ppc__) && !defined(__PPC__) && !defined(__PPC64__)
		#define NOMAD_VA_COPY_ENABLED 1
	#else
		#define NOMAD_VA_COPY_ENABLED 0
	#endif
#endif

#if !defined(NOMAD_VSPRINTF_OVERRIDE)
	#define NOMAD_VSNPRINTF_OVERRIDE stbsp_vsnprintf
#endif

#define NOMAD_BIT( x ) ( 1 << ( x ) )

#if defined(NOMAD_COMPILER_GCC)
	// We can't use GCC 4's __builtin_offsetof because it mistakenly complains about non-PODs that are really PODs.
	//    #define NOMAD_offsetof( type, member ) ((size_t)(((uintptr_t)&reinterpret_cast<const volatile char&>((((type*)65536)->member))) - 65536))
	#define NOMAD_offsetof( type, member ) ((size_t)&(((type*)0)->member))
#else
	#define NOMAD_offsetof( type, member ) offsetof( type, member )
#endif

#ifdef offsetof
#undef offsetof
#define offsetof( type, member ) NOMAD_offsetof( type, member )
#endif

#define NOMAD_EXPORT_DEMANGLE extern "C"

NOMAD_EXPORT_DEMANGLE void NOMAD_AssertionFailure( const char *pAssertion, const char *pFileName, unsigned long nLineNumber );

#if defined(assert)
#undef assert
#endif
#define assert( x ) NOMAD_Assert( x )
#define NOMAD_Assert( x ) ( ( x ) ? (void)0 : NOMAD_AssertionFailure( #x, __FILE__, __LINE__ ) )


#define NOMAD_PAD( nBase, nAlignment ) ( ( ( nBase ) + ( nAlignment ) - 1 ) & ~( ( nAlignment ) - 1 ) )
#define NOMAD_Vsnprintf NOMAD_VSNPRINTF_OVERRIDE

#define NOMAD_INT8_MIN INT8_MIN
#define NOMAD_INT16_MIN INT16_MIN
#define NOMAD_INT32_MIN INT32_MIN
#define NOMAD_INT64_MIN INT64_MIN
#define NOMAD_UINT8_MIN UINT8_MIN
#define NOMAD_UINT16_MIN UINT16_MIN
#define NOMAD_UINT32_MIN UINT32_MIN
#define NOMAD_UINT64_MIN UINT64_MIN
#define NOMAD_INT8_MAX INT8_MAX
#define NOMAD_INT16_MAX INT16_MAX
#define NOMAD_INT32_MAX INT32_MAX
#define NOMAD_INT64_MAX INT64_MAX
#define NOMAD_UINT8_MAX UINT8_MAX
#define NOMAD_UINT16_MAX UINT16_MAX
#define NOMAD_UINT32_MAX UINT32_MAX
#define NOMAD_UINT64_MAX UINT64_MAX

#endif