// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:0,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:True,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:7515,x:39034,y:33079,varname:node_7515,prsc:2|diff-3980-OUT,spec-6052-OUT,gloss-7877-OUT,normal-8232-OUT;n:type:ShaderForge.SFN_Tex2d,id:3789,x:33527,y:33258,ptovrint:False,ptlb:MaskMixer(rgb)_AO(a),ptin:_MaskMixerrgb_AOa,varname:node_894,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:171,x:34234,y:33638,ptovrint:False,ptlb:Mask1_color,ptin:_Mask1_color,varname:node_9461,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.007352948,c3:0.007352948,c4:1;n:type:ShaderForge.SFN_Multiply,id:6470,x:34090,y:33751,varname:node_6470,prsc:2|A-3789-R,B-9935-OUT;n:type:ShaderForge.SFN_Color,id:1050,x:34260,y:33830,ptovrint:False,ptlb:Mask2_color,ptin:_Mask2_color,varname:_Mask1_color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.05082181,c2:0.6911765,c3:0.1082329,c4:1;n:type:ShaderForge.SFN_Color,id:7405,x:34276,y:34046,ptovrint:False,ptlb:Mask3_color,ptin:_Mask3_color,varname:_Mask2_color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.04974049,c2:0.3912004,c3:0.6764706,c4:1;n:type:ShaderForge.SFN_Add,id:3814,x:35084,y:33754,varname:node_3814,prsc:2|A-8549-OUT,B-9889-OUT;n:type:ShaderForge.SFN_Add,id:9891,x:35253,y:33770,varname:node_9891,prsc:2|A-3814-OUT,B-3882-OUT;n:type:ShaderForge.SFN_Multiply,id:8549,x:34869,y:33715,varname:node_8549,prsc:2|A-878-OUT,B-6470-OUT;n:type:ShaderForge.SFN_Multiply,id:3340,x:34111,y:33968,varname:node_3340,prsc:2|A-3789-G,B-9959-OUT;n:type:ShaderForge.SFN_Multiply,id:6174,x:34135,y:34185,varname:node_6174,prsc:2|A-3789-B,B-6346-OUT;n:type:ShaderForge.SFN_Tex2d,id:9791,x:32924,y:33826,ptovrint:False,ptlb:Detail_Map1,ptin:_Detail_Map1,varname:node_47,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1072,x:32924,y:34018,ptovrint:False,ptlb:Detail_Map2,ptin:_Detail_Map2,varname:_Detail_Map2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4210,x:32905,y:34246,ptovrint:False,ptlb:Detail_Map3,ptin:_Detail_Map3,varname:_Detail_Map3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9889,x:34869,y:33900,varname:node_9889,prsc:2|A-8341-OUT,B-3340-OUT;n:type:ShaderForge.SFN_Multiply,id:3882,x:34869,y:34113,varname:node_3882,prsc:2|A-2421-OUT,B-6174-OUT;n:type:ShaderForge.SFN_Tex2d,id:1477,x:35331,y:33881,ptovrint:False,ptlb:Detail_Norm1,ptin:_Detail_Norm1,varname:node_2018,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:6939,x:35295,y:34101,ptovrint:False,ptlb:Detail_Norm2,ptin:_Detail_Norm2,varname:_Detail_Norm2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:9865,x:35249,y:34310,ptovrint:False,ptlb:Detail_Norm3,ptin:_Detail_Norm3,varname:_Detail_Norm3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:60,x:36166,y:33844,varname:node_60,prsc:2|A-2741-OUT,B-3789-R;n:type:ShaderForge.SFN_Multiply,id:1864,x:36166,y:34019,varname:node_1864,prsc:2|A-3182-OUT,B-3789-G;n:type:ShaderForge.SFN_Multiply,id:8963,x:36155,y:34179,varname:node_8963,prsc:2|A-4283-OUT,B-3789-B;n:type:ShaderForge.SFN_Multiply,id:9425,x:35278,y:32421,varname:node_9425,prsc:2|A-3789-R,B-7483-OUT;n:type:ShaderForge.SFN_Multiply,id:463,x:35311,y:32606,varname:node_463,prsc:2|A-3789-G,B-11-OUT;n:type:ShaderForge.SFN_Multiply,id:8737,x:35293,y:32778,varname:node_8737,prsc:2|A-3789-B,B-172-OUT;n:type:ShaderForge.SFN_Slider,id:1974,x:34275,y:32274,ptovrint:False,ptlb:DetailGloss1,ptin:_DetailGloss1,varname:node_3509,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6407767,max:1;n:type:ShaderForge.SFN_Slider,id:1557,x:34261,y:32462,ptovrint:False,ptlb:DetailGloss2,ptin:_DetailGloss2,varname:_DetailGloss2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7112322,max:1;n:type:ShaderForge.SFN_Slider,id:7524,x:34275,y:32659,ptovrint:False,ptlb:DetailGloss3,ptin:_DetailGloss3,varname:_DetailGloss3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4144778,max:1;n:type:ShaderForge.SFN_Add,id:1352,x:35601,y:32479,varname:node_1352,prsc:2|A-9425-OUT,B-463-OUT;n:type:ShaderForge.SFN_Add,id:7930,x:36005,y:32819,varname:node_7930,prsc:2|A-3708-OUT,B-8737-OUT;n:type:ShaderForge.SFN_Add,id:7766,x:36357,y:34179,varname:node_7766,prsc:2|A-1027-OUT,B-8963-OUT;n:type:ShaderForge.SFN_Color,id:7921,x:34040,y:32964,ptovrint:False,ptlb:DetailSpecCol1,ptin:_DetailSpecCol1,varname:node_1873,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:3829,x:34151,y:33224,ptovrint:False,ptlb:DetailSpecCol2,ptin:_DetailSpecCol2,varname:_DetailSpecCol2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1172414,c2:1,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:7545,x:34104,y:33446,ptovrint:False,ptlb:DetailSpecCol3,ptin:_DetailSpecCol3,varname:_DetailSpecCol3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:5474,x:34372,y:33012,varname:node_5474,prsc:2|A-7921-RGB,B-3789-R;n:type:ShaderForge.SFN_Multiply,id:4709,x:34436,y:33165,varname:node_4709,prsc:2|A-3829-RGB,B-3789-G;n:type:ShaderForge.SFN_Multiply,id:3094,x:34436,y:33392,varname:node_3094,prsc:2|A-7545-RGB,B-3789-B;n:type:ShaderForge.SFN_Add,id:8612,x:35079,y:33133,varname:node_8612,prsc:2|A-2846-OUT,B-8937-OUT;n:type:ShaderForge.SFN_Add,id:2264,x:35409,y:33288,varname:node_2264,prsc:2|A-8612-OUT,B-2799-OUT;n:type:ShaderForge.SFN_Add,id:1027,x:36448,y:33906,varname:node_1027,prsc:2|A-60-OUT,B-1864-OUT;n:type:ShaderForge.SFN_Color,id:2261,x:36701,y:33066,ptovrint:False,ptlb:Base_Color,ptin:_Base_Color,varname:node_9123,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:33333,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:6678,x:36918,y:33182,varname:node_6678,prsc:2|A-2261-RGB,B-5404-OUT;n:type:ShaderForge.SFN_Clamp01,id:5090,x:37071,y:33513,varname:node_5090,prsc:2|IN-7287-OUT;n:type:ShaderForge.SFN_Clamp01,id:5404,x:35565,y:33641,varname:node_5404,prsc:2|IN-9891-OUT;n:type:ShaderForge.SFN_Tex2d,id:9331,x:36892,y:32927,ptovrint:False,ptlb:Base_Texture,ptin:_Base_Texture,varname:node_2478,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5158,x:37266,y:33983,ptovrint:False,ptlb:Base_normal_Map,ptin:_Base_normal_Map,varname:node_4698,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_NormalBlend,id:8148,x:37591,y:33789,varname:node_8148,prsc:2|BSE-7766-OUT,DTL-5158-RGB;n:type:ShaderForge.SFN_Blend,id:9638,x:37229,y:33087,varname:node_9638,prsc:2,blmd:1,clmp:True|SRC-9331-RGB,DST-6678-OUT;n:type:ShaderForge.SFN_Multiply,id:4260,x:37467,y:33000,varname:node_4260,prsc:2|A-31-OUT,B-9638-OUT;n:type:ShaderForge.SFN_Lerp,id:9935,x:33788,y:33791,varname:node_9935,prsc:2|A-4318-OUT,B-9791-RGB,T-1642-OUT;n:type:ShaderForge.SFN_Vector3,id:4318,x:33509,y:33697,varname:node_4318,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_Slider,id:1642,x:32516,y:34580,ptovrint:False,ptlb:Detail1_strength,ptin:_Detail1_strength,cmnt:DETAIL MAP 1 - RED Channel,varname:node_8175,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5631068,max:1;n:type:ShaderForge.SFN_Vector3,id:2900,x:33624,y:34006,varname:node_2900,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_Slider,id:5167,x:32516,y:34700,ptovrint:False,ptlb:Detail2_strength,ptin:_Detail2_strength,cmnt:DETAIL MAP 2 - GREEN Channel,varname:_Detail1_strength_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Lerp,id:9959,x:33899,y:34025,varname:node_9959,prsc:2|A-2900-OUT,B-1072-RGB,T-5167-OUT;n:type:ShaderForge.SFN_Lerp,id:6346,x:33887,y:34298,varname:node_6346,prsc:2|A-9260-OUT,B-4210-RGB,T-8292-OUT;n:type:ShaderForge.SFN_Vector3,id:9260,x:33624,y:34291,varname:node_9260,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_Slider,id:8292,x:32503,y:34832,ptovrint:False,ptlb:Detail3_strength,ptin:_Detail3_strength,cmnt:DETAIL MAP 3 - BLUE Channel,varname:_Detail2_strength_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8241895,max:1;n:type:ShaderForge.SFN_Multiply,id:31,x:36823,y:32694,varname:node_31,prsc:2|A-3789-A,B-1251-OUT;n:type:ShaderForge.SFN_ValueProperty,id:330,x:36298,y:32955,ptovrint:False,ptlb:AO_boost,ptin:_AO_boost,varname:node_144,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Vector3,id:4344,x:35682,y:33785,varname:node_4344,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Lerp,id:2741,x:35940,y:33709,varname:node_2741,prsc:2|A-4344-OUT,B-7809-OUT,T-1642-OUT;n:type:ShaderForge.SFN_Lerp,id:3182,x:35969,y:33886,varname:node_3182,prsc:2|A-4344-OUT,B-510-OUT,T-5167-OUT;n:type:ShaderForge.SFN_Lerp,id:4283,x:35969,y:34107,varname:node_4283,prsc:2|A-4344-OUT,B-1757-OUT,T-8292-OUT;n:type:ShaderForge.SFN_Multiply,id:1251,x:36610,y:32755,varname:node_1251,prsc:2|A-3789-A,B-330-OUT;n:type:ShaderForge.SFN_NormalVector,id:7060,x:36197,y:32293,prsc:2,pt:False;n:type:ShaderForge.SFN_Abs,id:5613,x:36511,y:32389,varname:node_5613,prsc:2|IN-7060-OUT;n:type:ShaderForge.SFN_Multiply,id:7706,x:36760,y:32342,varname:node_7706,prsc:2|A-7060-OUT,B-5613-OUT;n:type:ShaderForge.SFN_Lerp,id:5668,x:38145,y:32985,varname:node_5668,prsc:2|A-4260-OUT,B-6892-OUT,T-9525-OUT;n:type:ShaderForge.SFN_Multiply,id:6892,x:37873,y:32766,varname:node_6892,prsc:2|A-5063-OUT,B-2649-RGB;n:type:ShaderForge.SFN_Color,id:2649,x:37224,y:32797,ptovrint:False,ptlb:Dirt_Color,ptin:_Dirt_Color,varname:node_2649,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:7322,x:38054,y:33331,varname:node_7322,prsc:2|A-3481-OUT,B-1576-RGB,T-9525-OUT;n:type:ShaderForge.SFN_Color,id:1576,x:37569,y:33366,ptovrint:False,ptlb:Dirt_SpecCol,ptin:_Dirt_SpecCol,varname:node_1576,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.128,c2:0.134,c3:0.147,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:3028,x:37339,y:33564,ptovrint:False,ptlb:Dirt_Gloss,ptin:_Dirt_Gloss,varname:node_3028,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Lerp,id:7536,x:38054,y:33456,varname:node_7536,prsc:2|A-5090-OUT,B-4160-OUT,T-9525-OUT;n:type:ShaderForge.SFN_Clamp01,id:4160,x:37528,y:33520,varname:node_4160,prsc:2|IN-3028-OUT;n:type:ShaderForge.SFN_ComponentMask,id:571,x:37211,y:32476,varname:node_571,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-7706-OUT;n:type:ShaderForge.SFN_Multiply,id:9525,x:37796,y:33113,varname:node_9525,prsc:2|A-571-OUT,B-7586-OUT;n:type:ShaderForge.SFN_Add,id:5063,x:37703,y:32620,varname:node_5063,prsc:2|A-571-OUT,B-6542-OUT;n:type:ShaderForge.SFN_Multiply,id:7190,x:37807,y:33936,varname:node_7190,prsc:2|A-8148-OUT,B-243-OUT;n:type:ShaderForge.SFN_Vector3,id:243,x:37591,y:34050,varname:node_243,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_Tex2d,id:4230,x:35802,y:33490,ptovrint:False,ptlb:BaseSpecColGloss(a),ptin:_BaseSpecColGlossa,varname:node_4230,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Add,id:9136,x:36732,y:33358,varname:node_9136,prsc:2|A-2264-OUT,B-3026-OUT;n:type:ShaderForge.SFN_Add,id:7287,x:36863,y:33501,varname:node_7287,prsc:2|A-7930-OUT,B-9227-OUT;n:type:ShaderForge.SFN_Color,id:5210,x:35636,y:33364,ptovrint:False,ptlb:Base_SpecColor,ptin:_Base_SpecColor,varname:node_5210,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:0;n:type:ShaderForge.SFN_Multiply,id:3026,x:36436,y:33379,varname:node_3026,prsc:2|A-5210-RGB,B-4230-RGB;n:type:ShaderForge.SFN_Multiply,id:6719,x:36888,y:33706,varname:node_6719,prsc:2|A-4230-A,B-7559-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:7586,x:37591,y:33199,varname:node_7586,prsc:2,min:0,max:1|IN-7724-OUT;n:type:ShaderForge.SFN_OneMinus,id:6542,x:38078,y:33890,varname:node_6542,prsc:2|IN-7190-OUT;n:type:ShaderForge.SFN_Slider,id:7724,x:37189,y:33278,ptovrint:False,ptlb:Dirt_Top,ptin:_Dirt_Top,varname:node_7724,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:3566,x:36145,y:33724,ptovrint:False,ptlb:Base_Gloss,ptin:_Base_Gloss,varname:node_3566,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5555561,max:1;n:type:ShaderForge.SFN_Add,id:844,x:35706,y:34467,varname:node_844,prsc:2|A-1642-OUT,B-5167-OUT;n:type:ShaderForge.SFN_Add,id:8983,x:36026,y:34515,varname:node_8983,prsc:2|A-844-OUT,B-8292-OUT;n:type:ShaderForge.SFN_Lerp,id:878,x:34635,y:33676,varname:node_878,prsc:2|A-6078-OUT,B-171-RGB,T-1642-OUT;n:type:ShaderForge.SFN_Lerp,id:8341,x:34601,y:33907,varname:node_8341,prsc:2|A-6078-OUT,B-1050-RGB,T-5167-OUT;n:type:ShaderForge.SFN_Lerp,id:2421,x:34625,y:34087,varname:node_2421,prsc:2|A-6078-OUT,B-7405-RGB,T-8292-OUT;n:type:ShaderForge.SFN_Vector3,id:6078,x:34420,y:33774,varname:node_6078,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_Divide,id:3708,x:35877,y:32566,varname:node_3708,prsc:2|A-1352-OUT,B-9856-OUT;n:type:ShaderForge.SFN_Vector1,id:9856,x:35556,y:32656,varname:node_9856,prsc:2,v1:2;n:type:ShaderForge.SFN_Divide,id:9227,x:36172,y:33022,varname:node_9227,prsc:2|A-7930-OUT,B-9856-OUT;n:type:ShaderForge.SFN_Lerp,id:4561,x:38269,y:33533,varname:node_4561,prsc:2|A-7536-OUT,B-6719-OUT,T-3566-OUT;n:type:ShaderForge.SFN_Multiply,id:7483,x:34856,y:32291,varname:node_7483,prsc:2|A-1974-OUT,B-1642-OUT;n:type:ShaderForge.SFN_Multiply,id:11,x:34856,y:32494,varname:node_11,prsc:2|A-1557-OUT,B-5167-OUT;n:type:ShaderForge.SFN_Multiply,id:172,x:34872,y:32707,varname:node_172,prsc:2|A-7524-OUT,B-8292-OUT;n:type:ShaderForge.SFN_Clamp01,id:2985,x:38529,y:33389,varname:node_2985,prsc:2|IN-4561-OUT;n:type:ShaderForge.SFN_Multiply,id:2846,x:34719,y:32995,varname:node_2846,prsc:2|A-5474-OUT,B-1642-OUT;n:type:ShaderForge.SFN_Multiply,id:8937,x:34719,y:33207,varname:node_8937,prsc:2|A-4709-OUT,B-5167-OUT;n:type:ShaderForge.SFN_Multiply,id:2799,x:34733,y:33371,varname:node_2799,prsc:2|A-3094-OUT,B-8292-OUT;n:type:ShaderForge.SFN_Clamp01,id:3481,x:37037,y:33372,varname:node_3481,prsc:2|IN-9136-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4624,x:35513,y:33948,ptovrint:False,ptlb:Norm1Strength,ptin:_Norm1Strength,varname:node_4624,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:5446,x:35484,y:34172,ptovrint:False,ptlb:Norm2Strength,ptin:_Norm2Strength,varname:_Norm1Strength_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:4872,x:35460,y:34361,ptovrint:False,ptlb:Norm3Strength,ptin:_Norm3Strength,varname:_Norm2Strength_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Vector3,id:9377,x:35494,y:33836,varname:node_9377,prsc:2,v1:0.5,v2:0.5,v3:1;n:type:ShaderForge.SFN_Lerp,id:7809,x:35709,y:33886,varname:node_7809,prsc:2|A-9377-OUT,B-1477-RGB,T-4624-OUT;n:type:ShaderForge.SFN_Lerp,id:510,x:35709,y:34027,varname:node_510,prsc:2|A-9377-OUT,B-6939-RGB,T-5446-OUT;n:type:ShaderForge.SFN_Lerp,id:1757,x:35724,y:34250,varname:node_1757,prsc:2|A-9377-OUT,B-9865-RGB,T-4872-OUT;n:type:ShaderForge.SFN_Multiply,id:3980,x:38688,y:32930,varname:node_3980,prsc:2|A-5668-OUT,B-3789-A;n:type:ShaderForge.SFN_Multiply,id:7877,x:38792,y:33320,varname:node_7877,prsc:2|A-3789-A,B-2985-OUT;n:type:ShaderForge.SFN_Multiply,id:6052,x:38688,y:33105,varname:node_6052,prsc:2|A-3789-A,B-7322-OUT;n:type:ShaderForge.SFN_Normalize,id:8232,x:37988,y:33699,varname:node_8232,prsc:2|IN-8148-OUT;n:type:ShaderForge.SFN_Add,id:7559,x:36551,y:33650,varname:node_7559,prsc:2|A-9227-OUT,B-3566-OUT;proporder:2261-9331-5210-4230-5158-3566-3789-1642-9791-171-7921-1974-1477-4624-5167-1072-1050-3829-1557-6939-5446-8292-4210-7405-7545-7524-9865-4872-330-7724-2649-1576-3028;pass:END;sub:END;*/

Shader "Custom/MegaDetailShader" {
    Properties {
        _Base_Color ("Base_Color", Color) = (33333,1,1,1)
        _Base_Texture ("Base_Texture", 2D) = "white" {}
        _Base_SpecColor ("Base_SpecColor", Color) = (0,0,0,0)
        _BaseSpecColGlossa ("BaseSpecColGloss(a)", 2D) = "black" {}
        _Base_normal_Map ("Base_normal_Map", 2D) = "bump" {}
        _Base_Gloss ("Base_Gloss", Range(0, 1)) = 0.5555561
        _MaskMixerrgb_AOa ("MaskMixer(rgb)_AO(a)", 2D) = "white" {}
        _Detail1_strength ("Detail1_strength", Range(0, 1)) = 0.5631068
        _Detail_Map1 ("Detail_Map1", 2D) = "gray" {}
        _Mask1_color ("Mask1_color", Color) = (1,0.007352948,0.007352948,1)
        _DetailSpecCol1 ("DetailSpecCol1", Color) = (1,0,0,1)
        _DetailGloss1 ("DetailGloss1", Range(0, 1)) = 0.6407767
        _Detail_Norm1 ("Detail_Norm1", 2D) = "bump" {}
        _Norm1Strength ("Norm1Strength", Float ) = 1
        _Detail2_strength ("Detail2_strength", Range(0, 1)) = 0.5
        _Detail_Map2 ("Detail_Map2", 2D) = "gray" {}
        _Mask2_color ("Mask2_color", Color) = (0.05082181,0.6911765,0.1082329,1)
        _DetailSpecCol2 ("DetailSpecCol2", Color) = (0.1172414,1,0,1)
        _DetailGloss2 ("DetailGloss2", Range(0, 1)) = 0.7112322
        _Detail_Norm2 ("Detail_Norm2", 2D) = "bump" {}
        _Norm2Strength ("Norm2Strength", Float ) = 1
        _Detail3_strength ("Detail3_strength", Range(0, 1)) = 0.8241895
        _Detail_Map3 ("Detail_Map3", 2D) = "gray" {}
        _Mask3_color ("Mask3_color", Color) = (0.04974049,0.3912004,0.6764706,1)
        _DetailSpecCol3 ("DetailSpecCol3", Color) = (0,0,1,1)
        _DetailGloss3 ("DetailGloss3", Range(0, 1)) = 0.4144778
        _Detail_Norm3 ("Detail_Norm3", 2D) = "bump" {}
        _Norm3Strength ("Norm3Strength", Float ) = 1
        _AO_boost ("AO_boost", Float ) = 1
        _Dirt_Top ("Dirt_Top", Range(0, 1)) = 0
        _Dirt_Color ("Dirt_Color", Color) = (1,1,1,1)
        _Dirt_SpecCol ("Dirt_SpecCol", Color) = (0.128,0.134,0.147,1)
        _Dirt_Gloss ("Dirt_Gloss", Float ) = 0.1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MaskMixerrgb_AOa; uniform float4 _MaskMixerrgb_AOa_ST;
            uniform float4 _Mask1_color;
            uniform float4 _Mask2_color;
            uniform float4 _Mask3_color;
            uniform sampler2D _Detail_Map1; uniform float4 _Detail_Map1_ST;
            uniform sampler2D _Detail_Map2; uniform float4 _Detail_Map2_ST;
            uniform sampler2D _Detail_Map3; uniform float4 _Detail_Map3_ST;
            uniform sampler2D _Detail_Norm1; uniform float4 _Detail_Norm1_ST;
            uniform sampler2D _Detail_Norm2; uniform float4 _Detail_Norm2_ST;
            uniform sampler2D _Detail_Norm3; uniform float4 _Detail_Norm3_ST;
            uniform float _DetailGloss1;
            uniform float _DetailGloss2;
            uniform float _DetailGloss3;
            uniform float4 _DetailSpecCol1;
            uniform float4 _DetailSpecCol2;
            uniform float4 _DetailSpecCol3;
            uniform float4 _Base_Color;
            uniform sampler2D _Base_Texture; uniform float4 _Base_Texture_ST;
            uniform sampler2D _Base_normal_Map; uniform float4 _Base_normal_Map_ST;
            uniform float _Detail1_strength;
            uniform float _Detail2_strength;
            uniform float _Detail3_strength;
            uniform float _AO_boost;
            uniform float4 _Dirt_Color;
            uniform float4 _Dirt_SpecCol;
            uniform float _Dirt_Gloss;
            uniform sampler2D _BaseSpecColGlossa; uniform float4 _BaseSpecColGlossa_ST;
            uniform float4 _Base_SpecColor;
            uniform float _Dirt_Top;
            uniform float _Base_Gloss;
            uniform float _Norm1Strength;
            uniform float _Norm2Strength;
            uniform float _Norm3Strength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 node_4344 = float3(0,0,1);
                float3 node_9377 = float3(0.5,0.5,1);
                float3 _Detail_Norm1_var = UnpackNormal(tex2D(_Detail_Norm1,TRANSFORM_TEX(i.uv0, _Detail_Norm1)));
                float4 _MaskMixerrgb_AOa_var = tex2D(_MaskMixerrgb_AOa,TRANSFORM_TEX(i.uv0, _MaskMixerrgb_AOa));
                float3 _Detail_Norm2_var = UnpackNormal(tex2D(_Detail_Norm2,TRANSFORM_TEX(i.uv0, _Detail_Norm2)));
                float3 _Detail_Norm3_var = UnpackNormal(tex2D(_Detail_Norm3,TRANSFORM_TEX(i.uv0, _Detail_Norm3)));
                float3 _Base_normal_Map_var = UnpackNormal(tex2D(_Base_normal_Map,TRANSFORM_TEX(i.uv0, _Base_normal_Map)));
                float3 node_8148_nrm_base = (((lerp(node_4344,lerp(node_9377,_Detail_Norm1_var.rgb,_Norm1Strength),_Detail1_strength)*_MaskMixerrgb_AOa_var.r)+(lerp(node_4344,lerp(node_9377,_Detail_Norm2_var.rgb,_Norm2Strength),_Detail2_strength)*_MaskMixerrgb_AOa_var.g))+(lerp(node_4344,lerp(node_9377,_Detail_Norm3_var.rgb,_Norm3Strength),_Detail3_strength)*_MaskMixerrgb_AOa_var.b)) + float3(0,0,1);
                float3 node_8148_nrm_detail = _Base_normal_Map_var.rgb * float3(-1,-1,1);
                float3 node_8148_nrm_combined = node_8148_nrm_base*dot(node_8148_nrm_base, node_8148_nrm_detail)/node_8148_nrm_base.z - node_8148_nrm_detail;
                float3 node_8148 = node_8148_nrm_combined;
                float3 normalLocal = normalize(node_8148);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float node_9856 = 2.0;
                float node_7930 = ((((_MaskMixerrgb_AOa_var.r*(_DetailGloss1*_Detail1_strength))+(_MaskMixerrgb_AOa_var.g*(_DetailGloss2*_Detail2_strength)))/node_9856)+(_MaskMixerrgb_AOa_var.b*(_DetailGloss3*_Detail3_strength)));
                float node_9227 = (node_7930/node_9856);
                float node_571 = (i.normalDir*abs(i.normalDir)).g;
                float node_9525 = (node_571*clamp(_Dirt_Top,0,1));
                float4 _BaseSpecColGlossa_var = tex2D(_BaseSpecColGlossa,TRANSFORM_TEX(i.uv0, _BaseSpecColGlossa));
                float gloss = (_MaskMixerrgb_AOa_var.a*saturate(lerp(lerp(saturate((node_7930+node_9227)),saturate(_Dirt_Gloss),node_9525),(_BaseSpecColGlossa_var.a*(node_9227+_Base_Gloss)),_Base_Gloss)));
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 specularColor = (_MaskMixerrgb_AOa_var.a*lerp(saturate((((((_DetailSpecCol1.rgb*_MaskMixerrgb_AOa_var.r)*_Detail1_strength)+((_DetailSpecCol2.rgb*_MaskMixerrgb_AOa_var.g)*_Detail2_strength))+((_DetailSpecCol3.rgb*_MaskMixerrgb_AOa_var.b)*_Detail3_strength))+(_Base_SpecColor.rgb*_BaseSpecColGlossa_var.rgb))),_Dirt_SpecCol.rgb,node_9525));
                float specularMonochrome;
                float4 _Base_Texture_var = tex2D(_Base_Texture,TRANSFORM_TEX(i.uv0, _Base_Texture));
                float3 node_6078 = float3(1,1,1);
                float4 _Detail_Map1_var = tex2D(_Detail_Map1,TRANSFORM_TEX(i.uv0, _Detail_Map1));
                float4 _Detail_Map2_var = tex2D(_Detail_Map2,TRANSFORM_TEX(i.uv0, _Detail_Map2));
                float4 _Detail_Map3_var = tex2D(_Detail_Map3,TRANSFORM_TEX(i.uv0, _Detail_Map3));
                float3 diffuseColor = (lerp(((_MaskMixerrgb_AOa_var.a*(_MaskMixerrgb_AOa_var.a*_AO_boost))*saturate((_Base_Texture_var.rgb*(_Base_Color.rgb*saturate((((lerp(node_6078,_Mask1_color.rgb,_Detail1_strength)*(_MaskMixerrgb_AOa_var.r*lerp(float3(1,1,1),_Detail_Map1_var.rgb,_Detail1_strength)))+(lerp(node_6078,_Mask2_color.rgb,_Detail2_strength)*(_MaskMixerrgb_AOa_var.g*lerp(float3(1,1,1),_Detail_Map2_var.rgb,_Detail2_strength))))+(lerp(node_6078,_Mask3_color.rgb,_Detail3_strength)*(_MaskMixerrgb_AOa_var.b*lerp(float3(1,1,1),_Detail_Map3_var.rgb,_Detail3_strength))))))))),((node_571+(1.0 - (node_8148*float3(0,1,0))))*_Dirt_Color.rgb),node_9525)*_MaskMixerrgb_AOa_var.a); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);
                if (IsGammaSpace())
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                specularPBL = max(0, specularPBL * NdotL);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz)*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MaskMixerrgb_AOa; uniform float4 _MaskMixerrgb_AOa_ST;
            uniform float4 _Mask1_color;
            uniform float4 _Mask2_color;
            uniform float4 _Mask3_color;
            uniform sampler2D _Detail_Map1; uniform float4 _Detail_Map1_ST;
            uniform sampler2D _Detail_Map2; uniform float4 _Detail_Map2_ST;
            uniform sampler2D _Detail_Map3; uniform float4 _Detail_Map3_ST;
            uniform sampler2D _Detail_Norm1; uniform float4 _Detail_Norm1_ST;
            uniform sampler2D _Detail_Norm2; uniform float4 _Detail_Norm2_ST;
            uniform sampler2D _Detail_Norm3; uniform float4 _Detail_Norm3_ST;
            uniform float _DetailGloss1;
            uniform float _DetailGloss2;
            uniform float _DetailGloss3;
            uniform float4 _DetailSpecCol1;
            uniform float4 _DetailSpecCol2;
            uniform float4 _DetailSpecCol3;
            uniform float4 _Base_Color;
            uniform sampler2D _Base_Texture; uniform float4 _Base_Texture_ST;
            uniform sampler2D _Base_normal_Map; uniform float4 _Base_normal_Map_ST;
            uniform float _Detail1_strength;
            uniform float _Detail2_strength;
            uniform float _Detail3_strength;
            uniform float _AO_boost;
            uniform float4 _Dirt_Color;
            uniform float4 _Dirt_SpecCol;
            uniform float _Dirt_Gloss;
            uniform sampler2D _BaseSpecColGlossa; uniform float4 _BaseSpecColGlossa_ST;
            uniform float4 _Base_SpecColor;
            uniform float _Dirt_Top;
            uniform float _Base_Gloss;
            uniform float _Norm1Strength;
            uniform float _Norm2Strength;
            uniform float _Norm3Strength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 node_4344 = float3(0,0,1);
                float3 node_9377 = float3(0.5,0.5,1);
                float3 _Detail_Norm1_var = UnpackNormal(tex2D(_Detail_Norm1,TRANSFORM_TEX(i.uv0, _Detail_Norm1)));
                float4 _MaskMixerrgb_AOa_var = tex2D(_MaskMixerrgb_AOa,TRANSFORM_TEX(i.uv0, _MaskMixerrgb_AOa));
                float3 _Detail_Norm2_var = UnpackNormal(tex2D(_Detail_Norm2,TRANSFORM_TEX(i.uv0, _Detail_Norm2)));
                float3 _Detail_Norm3_var = UnpackNormal(tex2D(_Detail_Norm3,TRANSFORM_TEX(i.uv0, _Detail_Norm3)));
                float3 _Base_normal_Map_var = UnpackNormal(tex2D(_Base_normal_Map,TRANSFORM_TEX(i.uv0, _Base_normal_Map)));
                float3 node_8148_nrm_base = (((lerp(node_4344,lerp(node_9377,_Detail_Norm1_var.rgb,_Norm1Strength),_Detail1_strength)*_MaskMixerrgb_AOa_var.r)+(lerp(node_4344,lerp(node_9377,_Detail_Norm2_var.rgb,_Norm2Strength),_Detail2_strength)*_MaskMixerrgb_AOa_var.g))+(lerp(node_4344,lerp(node_9377,_Detail_Norm3_var.rgb,_Norm3Strength),_Detail3_strength)*_MaskMixerrgb_AOa_var.b)) + float3(0,0,1);
                float3 node_8148_nrm_detail = _Base_normal_Map_var.rgb * float3(-1,-1,1);
                float3 node_8148_nrm_combined = node_8148_nrm_base*dot(node_8148_nrm_base, node_8148_nrm_detail)/node_8148_nrm_base.z - node_8148_nrm_detail;
                float3 node_8148 = node_8148_nrm_combined;
                float3 normalLocal = normalize(node_8148);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float node_9856 = 2.0;
                float node_7930 = ((((_MaskMixerrgb_AOa_var.r*(_DetailGloss1*_Detail1_strength))+(_MaskMixerrgb_AOa_var.g*(_DetailGloss2*_Detail2_strength)))/node_9856)+(_MaskMixerrgb_AOa_var.b*(_DetailGloss3*_Detail3_strength)));
                float node_9227 = (node_7930/node_9856);
                float node_571 = (i.normalDir*abs(i.normalDir)).g;
                float node_9525 = (node_571*clamp(_Dirt_Top,0,1));
                float4 _BaseSpecColGlossa_var = tex2D(_BaseSpecColGlossa,TRANSFORM_TEX(i.uv0, _BaseSpecColGlossa));
                float gloss = (_MaskMixerrgb_AOa_var.a*saturate(lerp(lerp(saturate((node_7930+node_9227)),saturate(_Dirt_Gloss),node_9525),(_BaseSpecColGlossa_var.a*(node_9227+_Base_Gloss)),_Base_Gloss)));
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 specularColor = (_MaskMixerrgb_AOa_var.a*lerp(saturate((((((_DetailSpecCol1.rgb*_MaskMixerrgb_AOa_var.r)*_Detail1_strength)+((_DetailSpecCol2.rgb*_MaskMixerrgb_AOa_var.g)*_Detail2_strength))+((_DetailSpecCol3.rgb*_MaskMixerrgb_AOa_var.b)*_Detail3_strength))+(_Base_SpecColor.rgb*_BaseSpecColGlossa_var.rgb))),_Dirt_SpecCol.rgb,node_9525));
                float specularMonochrome;
                float4 _Base_Texture_var = tex2D(_Base_Texture,TRANSFORM_TEX(i.uv0, _Base_Texture));
                float3 node_6078 = float3(1,1,1);
                float4 _Detail_Map1_var = tex2D(_Detail_Map1,TRANSFORM_TEX(i.uv0, _Detail_Map1));
                float4 _Detail_Map2_var = tex2D(_Detail_Map2,TRANSFORM_TEX(i.uv0, _Detail_Map2));
                float4 _Detail_Map3_var = tex2D(_Detail_Map3,TRANSFORM_TEX(i.uv0, _Detail_Map3));
                float3 diffuseColor = (lerp(((_MaskMixerrgb_AOa_var.a*(_MaskMixerrgb_AOa_var.a*_AO_boost))*saturate((_Base_Texture_var.rgb*(_Base_Color.rgb*saturate((((lerp(node_6078,_Mask1_color.rgb,_Detail1_strength)*(_MaskMixerrgb_AOa_var.r*lerp(float3(1,1,1),_Detail_Map1_var.rgb,_Detail1_strength)))+(lerp(node_6078,_Mask2_color.rgb,_Detail2_strength)*(_MaskMixerrgb_AOa_var.g*lerp(float3(1,1,1),_Detail_Map2_var.rgb,_Detail2_strength))))+(lerp(node_6078,_Mask3_color.rgb,_Detail3_strength)*(_MaskMixerrgb_AOa_var.b*lerp(float3(1,1,1),_Detail_Map3_var.rgb,_Detail3_strength))))))))),((node_571+(1.0 - (node_8148*float3(0,1,0))))*_Dirt_Color.rgb),node_9525)*_MaskMixerrgb_AOa_var.a); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);
                if (IsGammaSpace())
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                specularPBL = max(0, specularPBL * NdotL);
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
