#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class ToolWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Tool);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 15, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "GetColliderWidthLua", _m_GetColliderWidthLua_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetColliderHeightLua", _m_GetColliderHeightLua_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetLuaName", _m_GetLuaName_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToVector2", _m_ToVector2_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToVector3", _m_ToVector3_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetCollider2Ds", _m_GetCollider2Ds_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetRandomWords", _m_GetRandomWords_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetSpecialWords", _m_GetSpecialWords_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetSpecialWordsCount", _m_GetSpecialWordsCount_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "TypeWriter", _m_TypeWriter_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetBoxColliderAnimation", _m_SetBoxColliderAnimation_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddBabbleEvent", _m_AddBabbleEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddBabbleEventOneTime", _m_AddBabbleEventOneTime_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "BabbleEventTrigger", _m_BabbleEventTrigger_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Tool does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetColliderWidthLua_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Collider2D _collider = (UnityEngine.Collider2D)translator.GetObject(L, 1, typeof(UnityEngine.Collider2D));
                    
                        var gen_ret = Tool.GetColliderWidthLua( _collider );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetColliderHeightLua_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Collider2D _collider = (UnityEngine.Collider2D)translator.GetObject(L, 1, typeof(UnityEngine.Collider2D));
                    
                        var gen_ret = Tool.GetColliderHeightLua( _collider );
                        LuaAPI.lua_pushnumber(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetLuaName_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = Tool.GetLuaName( _path );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToVector2_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector3 _vector;translator.Get(L, 1, out _vector);
                    
                        var gen_ret = Tool.ToVector2( _vector );
                        translator.PushUnityEngineVector2(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToVector3_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector2 _vector;translator.Get(L, 1, out _vector);
                    
                        var gen_ret = Tool.ToVector3( _vector );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetCollider2Ds_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _gameObject = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                        var gen_ret = Tool.GetCollider2Ds( _gameObject );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetRandomWords_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = Tool.GetRandomWords(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetSpecialWords_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _id = LuaAPI.xlua_tointeger(L, 1);
                    
                        var gen_ret = Tool.GetSpecialWords( _id );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetSpecialWordsCount_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = Tool.GetSpecialWordsCount(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TypeWriter_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    TMPro.TMP_Text __Text = (TMPro.TMP_Text)translator.GetObject(L, 1, typeof(TMPro.TMP_Text));
                    string _words = LuaAPI.lua_tostring(L, 2);
                    float _time = (float)LuaAPI.lua_tonumber(L, 3);
                    System.Action _actionback = translator.GetDelegate<System.Action>(L, 4);
                    
                    Tool.TypeWriter( __Text, _words, _time, _actionback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetBoxColliderAnimation_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.BoxCollider2D _collider2D = (UnityEngine.BoxCollider2D)translator.GetObject(L, 1, typeof(UnityEngine.BoxCollider2D));
                    UnityEngine.Vector2 _targetSize;translator.Get(L, 2, out _targetSize);
                    UnityEngine.Vector2 _targetOffset;translator.Get(L, 3, out _targetOffset);
                    float _time = (float)LuaAPI.lua_tonumber(L, 4);
                    DG.Tweening.Tween _sizeTween;
                    DG.Tweening.Tween _offsetTween;
                    
                    Tool.SetBoxColliderAnimation( _collider2D, _targetSize, _targetOffset, _time, out _sizeTween, out _offsetTween );
                    translator.Push(L, _sizeTween);
                        
                    translator.Push(L, _offsetTween);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddBabbleEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    int _id = LuaAPI.xlua_tointeger(L, 1);
                    System.Action _action = translator.GetDelegate<System.Action>(L, 2);
                    
                    Tool.AddBabbleEvent( _id, _action );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddBabbleEventOneTime_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    int _id = LuaAPI.xlua_tointeger(L, 1);
                    System.Action _action = translator.GetDelegate<System.Action>(L, 2);
                    
                    Tool.AddBabbleEventOneTime( _id, _action );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BabbleEventTrigger_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _id = LuaAPI.xlua_tointeger(L, 1);
                    
                    Tool.BabbleEventTrigger( _id );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
