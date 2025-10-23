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
    public class DetectionManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(DetectionManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 11, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Raycast2D", _m_Raycast2D_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Raycast2DByTag", _m_Raycast2DByTag_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "OverlapBoxByTag", _m_OverlapBoxByTag_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Raycast2DOutHit", _m_Raycast2DOutHit_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Raycast2DNoLayer", _m_Raycast2DNoLayer_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Raycast2DNoLayerOutHit", _m_Raycast2DNoLayerOutHit_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "MultiRayGroundCheck", _m_MultiRayGroundCheck_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CircleCast", _m_CircleCast_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CircleCastOutHit", _m_CircleCastOutHit_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Draw", _m_Draw_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "DetectionManager does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Raycast2D_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector2 _origin;translator.Get(L, 1, out _origin);
                    UnityEngine.Vector2 _direction;translator.Get(L, 2, out _direction);
                    float _distance = (float)LuaAPI.lua_tonumber(L, 3);
                    string _layerMask = LuaAPI.lua_tostring(L, 4);
                    
                        var gen_ret = DetectionManager.Raycast2D( _origin, _direction, _distance, _layerMask );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Raycast2DByTag_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector2 _origin;translator.Get(L, 1, out _origin);
                    UnityEngine.Vector2 _direction;translator.Get(L, 2, out _direction);
                    float _distance = (float)LuaAPI.lua_tonumber(L, 3);
                    string _tag = LuaAPI.lua_tostring(L, 4);
                    
                        var gen_ret = DetectionManager.Raycast2DByTag( _origin, _direction, _distance, _tag );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OverlapBoxByTag_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector2 _center;translator.Get(L, 1, out _center);
                    UnityEngine.Vector2 _size;translator.Get(L, 2, out _size);
                    string _tag = LuaAPI.lua_tostring(L, 3);
                    
                        var gen_ret = DetectionManager.OverlapBoxByTag( _center, _size, _tag );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Raycast2DOutHit_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector2 _origin;translator.Get(L, 1, out _origin);
                    UnityEngine.Vector2 _direction;translator.Get(L, 2, out _direction);
                    float _distance = (float)LuaAPI.lua_tonumber(L, 3);
                    string _layerMask = LuaAPI.lua_tostring(L, 4);
                    UnityEngine.RaycastHit2D _hitInfo;
                    
                        var gen_ret = DetectionManager.Raycast2DOutHit( _origin, _direction, _distance, _layerMask, out _hitInfo );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    translator.Push(L, _hitInfo);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Raycast2DNoLayer_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector2 _origin;translator.Get(L, 1, out _origin);
                    UnityEngine.Vector2 _direction;translator.Get(L, 2, out _direction);
                    float _distance = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        var gen_ret = DetectionManager.Raycast2DNoLayer( _origin, _direction, _distance );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Raycast2DNoLayerOutHit_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector2 _origin;translator.Get(L, 1, out _origin);
                    UnityEngine.Vector2 _direction;translator.Get(L, 2, out _direction);
                    float _distance = (float)LuaAPI.lua_tonumber(L, 3);
                    UnityEngine.RaycastHit2D _hitInfo;
                    
                        var gen_ret = DetectionManager.Raycast2DNoLayerOutHit( _origin, _direction, _distance, out _hitInfo );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    translator.Push(L, _hitInfo);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_MultiRayGroundCheck_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Collider2D _collider = (UnityEngine.Collider2D)translator.GetObject(L, 1, typeof(UnityEngine.Collider2D));
                    int _rayCount = LuaAPI.xlua_tointeger(L, 2);
                    float _distance = (float)LuaAPI.lua_tonumber(L, 3);
                    string _groundLayer = LuaAPI.lua_tostring(L, 4);
                    
                        var gen_ret = DetectionManager.MultiRayGroundCheck( _collider, _rayCount, _distance, _groundLayer );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CircleCast_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector2 _center;translator.Get(L, 1, out _center);
                    float _radius = (float)LuaAPI.lua_tonumber(L, 2);
                    string _layerMask = LuaAPI.lua_tostring(L, 3);
                    
                        var gen_ret = DetectionManager.CircleCast( _center, _radius, _layerMask );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CircleCastOutHit_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector2 _center;translator.Get(L, 1, out _center);
                    float _radius = (float)LuaAPI.lua_tonumber(L, 2);
                    string _layerMask = LuaAPI.lua_tostring(L, 3);
                    UnityEngine.Collider2D _hit;
                    
                        var gen_ret = DetectionManager.CircleCastOutHit( _center, _radius, _layerMask, out _hit );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    translator.Push(L, _hit);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Draw_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    bool _ishit = LuaAPI.lua_toboolean(L, 1);
                    UnityEngine.Vector2 _position;translator.Get(L, 2, out _position);
                    UnityEngine.Vector2 _direction;translator.Get(L, 3, out _direction);
                    float _rayLength = (float)LuaAPI.lua_tonumber(L, 4);
                    
                    DetectionManager.Draw( _ishit, _position, _direction, _rayLength );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
