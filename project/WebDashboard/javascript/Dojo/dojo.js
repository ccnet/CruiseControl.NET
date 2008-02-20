/*
	Copyright (c) 2004-2006, The Dojo Foundation
	All Rights Reserved.

	Licensed under the Academic Free License version 2.1 or above OR the
	modified BSD license. For more information on Dojo licensing, see:

		http://dojotoolkit.org/community/licensing.shtml
*/

/*
	This is a compiled version of Dojo, built for deployment and not for
	development. To get an editable version, please visit:

		http://dojotoolkit.org

	for documentation and information on getting the source.
*/

if(typeof dojo=="undefined"){
var dj_global=this;
var dj_currentContext=this;
function dj_undef(_1,_2){
return (typeof (_2||dj_currentContext)[_1]=="undefined");
}
if(dj_undef("djConfig",this)){
var djConfig={};
}
if(dj_undef("dojo",this)){
var dojo={};
}
dojo.global=function(){
return dj_currentContext;
};
dojo.locale=djConfig.locale;
dojo.version={major:0,minor:4,patch:2,flag:"",revision:Number("$Rev: 7616 $".match(/[0-9]+/)[0]),toString:function(){
with(dojo.version){
return major+"."+minor+"."+patch+flag+" ("+revision+")";
}
}};
dojo.evalProp=function(_3,_4,_5){
if((!_4)||(!_3)){
return undefined;
}
if(!dj_undef(_3,_4)){
return _4[_3];
}
return (_5?(_4[_3]={}):undefined);
};
dojo.parseObjPath=function(_6,_7,_8){
var _9=(_7||dojo.global());
var _a=_6.split(".");
var _b=_a.pop();
for(var i=0,l=_a.length;i<l&&_9;i++){
_9=dojo.evalProp(_a[i],_9,_8);
}
return {obj:_9,prop:_b};
};
dojo.evalObjPath=function(_e,_f){
if(typeof _e!="string"){
return dojo.global();
}
if(_e.indexOf(".")==-1){
return dojo.evalProp(_e,dojo.global(),_f);
}
var ref=dojo.parseObjPath(_e,dojo.global(),_f);
if(ref){
return dojo.evalProp(ref.prop,ref.obj,_f);
}
return null;
};
dojo.errorToString=function(_11){
if(!dj_undef("message",_11)){
return _11.message;
}else{
if(!dj_undef("description",_11)){
return _11.description;
}else{
return _11;
}
}
};
dojo.raise=function(_12,_13){
if(_13){
_12=_12+": "+dojo.errorToString(_13);
}else{
_12=dojo.errorToString(_12);
}
try{
if(djConfig.isDebug){
dojo.hostenv.println("FATAL exception raised: "+_12);
}
}
catch(e){
}
throw _13||Error(_12);
};
dojo.debug=function(){
};
dojo.debugShallow=function(obj){
};
dojo.profile={start:function(){
},end:function(){
},stop:function(){
},dump:function(){
}};
function dj_eval(_15){
return dj_global.eval?dj_global.eval(_15):eval(_15);
}
dojo.unimplemented=function(_16,_17){
var _18="'"+_16+"' not implemented";
if(_17!=null){
_18+=" "+_17;
}
dojo.raise(_18);
};
dojo.deprecated=function(_19,_1a,_1b){
var _1c="DEPRECATED: "+_19;
if(_1a){
_1c+=" "+_1a;
}
if(_1b){
_1c+=" -- will be removed in version: "+_1b;
}
dojo.debug(_1c);
};
dojo.render=(function(){
function vscaffold(_1d,_1e){
var tmp={capable:false,support:{builtin:false,plugin:false},prefixes:_1d};
for(var i=0;i<_1e.length;i++){
tmp[_1e[i]]=false;
}
return tmp;
}
return {name:"",ver:dojo.version,os:{win:false,linux:false,osx:false},html:vscaffold(["html"],["ie","opera","khtml","safari","moz"]),svg:vscaffold(["svg"],["corel","adobe","batik"]),vml:vscaffold(["vml"],["ie"]),swf:vscaffold(["Swf","Flash","Mm"],["mm"]),swt:vscaffold(["Swt"],["ibm"])};
})();
dojo.hostenv=(function(){
var _21={isDebug:false,allowQueryConfig:false,baseScriptUri:"",baseRelativePath:"",libraryScriptUri:"",iePreventClobber:false,ieClobberMinimal:true,preventBackButtonFix:true,delayMozLoadingFix:false,searchIds:[],parseWidgets:true};
if(typeof djConfig=="undefined"){
djConfig=_21;
}else{
for(var _22 in _21){
if(typeof djConfig[_22]=="undefined"){
djConfig[_22]=_21[_22];
}
}
}
return {name_:"(unset)",version_:"(unset)",getName:function(){
return this.name_;
},getVersion:function(){
return this.version_;
},getText:function(uri){
dojo.unimplemented("getText","uri="+uri);
}};
})();
dojo.hostenv.getBaseScriptUri=function(){
if(djConfig.baseScriptUri.length){
return djConfig.baseScriptUri;
}
var uri=new String(djConfig.libraryScriptUri||djConfig.baseRelativePath);
if(!uri){
dojo.raise("Nothing returned by getLibraryScriptUri(): "+uri);
}
var _25=uri.lastIndexOf("/");
djConfig.baseScriptUri=djConfig.baseRelativePath;
return djConfig.baseScriptUri;
};
(function(){
var _26={pkgFileName:"__package__",loading_modules_:{},loaded_modules_:{},addedToLoadingCount:[],removedFromLoadingCount:[],inFlightCount:0,modulePrefixes_:{dojo:{name:"dojo",value:"src"}},setModulePrefix:function(_27,_28){
this.modulePrefixes_[_27]={name:_27,value:_28};
},moduleHasPrefix:function(_29){
var mp=this.modulePrefixes_;
return Boolean(mp[_29]&&mp[_29].value);
},getModulePrefix:function(_2b){
if(this.moduleHasPrefix(_2b)){
return this.modulePrefixes_[_2b].value;
}
return _2b;
},getTextStack:[],loadUriStack:[],loadedUris:[],post_load_:false,modulesLoadedListeners:[],unloadListeners:[],loadNotifying:false};
for(var _2c in _26){
dojo.hostenv[_2c]=_26[_2c];
}
})();
dojo.hostenv.loadPath=function(_2d,_2e,cb){
var uri;
if(_2d.charAt(0)=="/"||_2d.match(/^\w+:/)){
uri=_2d;
}else{
uri=this.getBaseScriptUri()+_2d;
}
if(djConfig.cacheBust&&dojo.render.html.capable){
uri+="?"+String(djConfig.cacheBust).replace(/\W+/g,"");
}
try{
return !_2e?this.loadUri(uri,cb):this.loadUriAndCheck(uri,_2e,cb);
}
catch(e){
dojo.debug(e);
return false;
}
};
dojo.hostenv.loadUri=function(uri,cb){
if(this.loadedUris[uri]){
return true;
}
var _33=this.getText(uri,null,true);
if(!_33){
return false;
}
this.loadedUris[uri]=true;
if(cb){
_33="("+_33+")";
}
var _34=dj_eval(_33);
if(cb){
cb(_34);
}
return true;
};
dojo.hostenv.loadUriAndCheck=function(uri,_36,cb){
var ok=true;
try{
ok=this.loadUri(uri,cb);
}
catch(e){
dojo.debug("failed loading ",uri," with error: ",e);
}
return Boolean(ok&&this.findModule(_36,false));
};
dojo.loaded=function(){
};
dojo.unloaded=function(){
};
dojo.hostenv.loaded=function(){
this.loadNotifying=true;
this.post_load_=true;
var mll=this.modulesLoadedListeners;
for(var x=0;x<mll.length;x++){
mll[x]();
}
this.modulesLoadedListeners=[];
this.loadNotifying=false;
dojo.loaded();
};
dojo.hostenv.unloaded=function(){
var mll=this.unloadListeners;
while(mll.length){
(mll.pop())();
}
dojo.unloaded();
};
dojo.addOnLoad=function(obj,_3d){
var dh=dojo.hostenv;
if(arguments.length==1){
dh.modulesLoadedListeners.push(obj);
}else{
if(arguments.length>1){
dh.modulesLoadedListeners.push(function(){
obj[_3d]();
});
}
}
if(dh.post_load_&&dh.inFlightCount==0&&!dh.loadNotifying){
dh.callLoaded();
}
};
dojo.addOnUnload=function(obj,_40){
var dh=dojo.hostenv;
if(arguments.length==1){
dh.unloadListeners.push(obj);
}else{
if(arguments.length>1){
dh.unloadListeners.push(function(){
obj[_40]();
});
}
}
};
dojo.hostenv.modulesLoaded=function(){
if(this.post_load_){
return;
}
if(this.loadUriStack.length==0&&this.getTextStack.length==0){
if(this.inFlightCount>0){
dojo.debug("files still in flight!");
return;
}
dojo.hostenv.callLoaded();
}
};
dojo.hostenv.callLoaded=function(){
if(typeof setTimeout=="object"||(djConfig["useXDomain"]&&dojo.render.html.opera)){
setTimeout("dojo.hostenv.loaded();",0);
}else{
dojo.hostenv.loaded();
}
};
dojo.hostenv.getModuleSymbols=function(_42){
var _43=_42.split(".");
for(var i=_43.length;i>0;i--){
var _45=_43.slice(0,i).join(".");
if((i==1)&&!this.moduleHasPrefix(_45)){
_43[0]="../"+_43[0];
}else{
var _46=this.getModulePrefix(_45);
if(_46!=_45){
_43.splice(0,i,_46);
break;
}
}
}
return _43;
};
dojo.hostenv._global_omit_module_check=false;
dojo.hostenv.loadModule=function(_47,_48,_49){
if(!_47){
return;
}
_49=this._global_omit_module_check||_49;
var _4a=this.findModule(_47,false);
if(_4a){
return _4a;
}
if(dj_undef(_47,this.loading_modules_)){
this.addedToLoadingCount.push(_47);
}
this.loading_modules_[_47]=1;
var _4b=_47.replace(/\./g,"/")+".js";
var _4c=_47.split(".");
var _4d=this.getModuleSymbols(_47);
var _4e=((_4d[0].charAt(0)!="/")&&!_4d[0].match(/^\w+:/));
var _4f=_4d[_4d.length-1];
var ok;
if(_4f=="*"){
_47=_4c.slice(0,-1).join(".");
while(_4d.length){
_4d.pop();
_4d.push(this.pkgFileName);
_4b=_4d.join("/")+".js";
if(_4e&&_4b.charAt(0)=="/"){
_4b=_4b.slice(1);
}
ok=this.loadPath(_4b,!_49?_47:null);
if(ok){
break;
}
_4d.pop();
}
}else{
_4b=_4d.join("/")+".js";
_47=_4c.join(".");
var _51=!_49?_47:null;
ok=this.loadPath(_4b,_51);
if(!ok&&!_48){
_4d.pop();
while(_4d.length){
_4b=_4d.join("/")+".js";
ok=this.loadPath(_4b,_51);
if(ok){
break;
}
_4d.pop();
_4b=_4d.join("/")+"/"+this.pkgFileName+".js";
if(_4e&&_4b.charAt(0)=="/"){
_4b=_4b.slice(1);
}
ok=this.loadPath(_4b,_51);
if(ok){
break;
}
}
}
if(!ok&&!_49){
dojo.raise("Could not load '"+_47+"'; last tried '"+_4b+"'");
}
}
if(!_49&&!this["isXDomain"]){
_4a=this.findModule(_47,false);
if(!_4a){
dojo.raise("symbol '"+_47+"' is not defined after loading '"+_4b+"'");
}
}
return _4a;
};
dojo.hostenv.startPackage=function(_52){
var _53=String(_52);
var _54=_53;
var _55=_52.split(/\./);
if(_55[_55.length-1]=="*"){
_55.pop();
_54=_55.join(".");
}
var _56=dojo.evalObjPath(_54,true);
this.loaded_modules_[_53]=_56;
this.loaded_modules_[_54]=_56;
return _56;
};
dojo.hostenv.findModule=function(_57,_58){
var lmn=String(_57);
if(this.loaded_modules_[lmn]){
return this.loaded_modules_[lmn];
}
if(_58){
dojo.raise("no loaded module named '"+_57+"'");
}
return null;
};
dojo.kwCompoundRequire=function(_5a){
var _5b=_5a["common"]||[];
var _5c=_5a[dojo.hostenv.name_]?_5b.concat(_5a[dojo.hostenv.name_]||[]):_5b.concat(_5a["default"]||[]);
for(var x=0;x<_5c.length;x++){
var _5e=_5c[x];
if(_5e.constructor==Array){
dojo.hostenv.loadModule.apply(dojo.hostenv,_5e);
}else{
dojo.hostenv.loadModule(_5e);
}
}
};
dojo.require=function(_5f){
dojo.hostenv.loadModule.apply(dojo.hostenv,arguments);
};
dojo.requireIf=function(_60,_61){
var _62=arguments[0];
if((_62===true)||(_62=="common")||(_62&&dojo.render[_62].capable)){
var _63=[];
for(var i=1;i<arguments.length;i++){
_63.push(arguments[i]);
}
dojo.require.apply(dojo,_63);
}
};
dojo.requireAfterIf=dojo.requireIf;
dojo.provide=function(_65){
return dojo.hostenv.startPackage.apply(dojo.hostenv,arguments);
};
dojo.registerModulePath=function(_66,_67){
return dojo.hostenv.setModulePrefix(_66,_67);
};
if(djConfig["modulePaths"]){
for(var param in djConfig["modulePaths"]){
dojo.registerModulePath(param,djConfig["modulePaths"][param]);
}
}
dojo.setModulePrefix=function(_68,_69){
dojo.deprecated("dojo.setModulePrefix(\""+_68+"\", \""+_69+"\")","replaced by dojo.registerModulePath","0.5");
return dojo.registerModulePath(_68,_69);
};
dojo.exists=function(obj,_6b){
var p=_6b.split(".");
for(var i=0;i<p.length;i++){
if(!obj[p[i]]){
return false;
}
obj=obj[p[i]];
}
return true;
};
dojo.hostenv.normalizeLocale=function(_6e){
var _6f=_6e?_6e.toLowerCase():dojo.locale;
if(_6f=="root"){
_6f="ROOT";
}
return _6f;
};
dojo.hostenv.searchLocalePath=function(_70,_71,_72){
_70=dojo.hostenv.normalizeLocale(_70);
var _73=_70.split("-");
var _74=[];
for(var i=_73.length;i>0;i--){
_74.push(_73.slice(0,i).join("-"));
}
_74.push(false);
if(_71){
_74.reverse();
}
for(var j=_74.length-1;j>=0;j--){
var loc=_74[j]||"ROOT";
var _78=_72(loc);
if(_78){
break;
}
}
};
dojo.hostenv.localesGenerated;
dojo.hostenv.registerNlsPrefix=function(){
dojo.registerModulePath("nls","nls");
};
dojo.hostenv.preloadLocalizations=function(){
if(dojo.hostenv.localesGenerated){
dojo.hostenv.registerNlsPrefix();
function preload(_79){
_79=dojo.hostenv.normalizeLocale(_79);
dojo.hostenv.searchLocalePath(_79,true,function(loc){
for(var i=0;i<dojo.hostenv.localesGenerated.length;i++){
if(dojo.hostenv.localesGenerated[i]==loc){
dojo["require"]("nls.dojo_"+loc);
return true;
}
}
return false;
});
}
preload();
var _7c=djConfig.extraLocale||[];
for(var i=0;i<_7c.length;i++){
preload(_7c[i]);
}
}
dojo.hostenv.preloadLocalizations=function(){
};
};
dojo.requireLocalization=function(_7e,_7f,_80,_81){
dojo.hostenv.preloadLocalizations();
var _82=dojo.hostenv.normalizeLocale(_80);
var _83=[_7e,"nls",_7f].join(".");
var _84="";
if(_81){
var _85=_81.split(",");
for(var i=0;i<_85.length;i++){
if(_82.indexOf(_85[i])==0){
if(_85[i].length>_84.length){
_84=_85[i];
}
}
}
if(!_84){
_84="ROOT";
}
}
var _87=_81?_84:_82;
var _88=dojo.hostenv.findModule(_83);
var _89=null;
if(_88){
if(djConfig.localizationComplete&&_88._built){
return;
}
var _8a=_87.replace("-","_");
var _8b=_83+"."+_8a;
_89=dojo.hostenv.findModule(_8b);
}
if(!_89){
_88=dojo.hostenv.startPackage(_83);
var _8c=dojo.hostenv.getModuleSymbols(_7e);
var _8d=_8c.concat("nls").join("/");
var _8e;
dojo.hostenv.searchLocalePath(_87,_81,function(loc){
var _90=loc.replace("-","_");
var _91=_83+"."+_90;
var _92=false;
if(!dojo.hostenv.findModule(_91)){
dojo.hostenv.startPackage(_91);
var _93=[_8d];
if(loc!="ROOT"){
_93.push(loc);
}
_93.push(_7f);
var _94=_93.join("/")+".js";
_92=dojo.hostenv.loadPath(_94,null,function(_95){
var _96=function(){
};
_96.prototype=_8e;
_88[_90]=new _96();
for(var j in _95){
_88[_90][j]=_95[j];
}
});
}else{
_92=true;
}
if(_92&&_88[_90]){
_8e=_88[_90];
}else{
_88[_90]=_8e;
}
if(_81){
return true;
}
});
}
if(_81&&_82!=_84){
_88[_82.replace("-","_")]=_88[_84.replace("-","_")];
}
};
(function(){
var _98=djConfig.extraLocale;
if(_98){
if(!_98 instanceof Array){
_98=[_98];
}
var req=dojo.requireLocalization;
dojo.requireLocalization=function(m,b,_9c,_9d){
req(m,b,_9c,_9d);
if(_9c){
return;
}
for(var i=0;i<_98.length;i++){
req(m,b,_98[i],_9d);
}
};
}
})();
}
if(typeof window!="undefined"){
(function(){
if(djConfig.allowQueryConfig){
var _9f=document.location.toString();
var _a0=_9f.split("?",2);
if(_a0.length>1){
var _a1=_a0[1];
var _a2=_a1.split("&");
for(var x in _a2){
var sp=_a2[x].split("=");
if((sp[0].length>9)&&(sp[0].substr(0,9)=="djConfig.")){
var opt=sp[0].substr(9);
try{
djConfig[opt]=eval(sp[1]);
}
catch(e){
djConfig[opt]=sp[1];
}
}
}
}
}
if(((djConfig["baseScriptUri"]=="")||(djConfig["baseRelativePath"]==""))&&(document&&document.getElementsByTagName)){
var _a6=document.getElementsByTagName("script");
var _a7=/(__package__|dojo|bootstrap1)\.js([\?\.]|$)/i;
for(var i=0;i<_a6.length;i++){
var src=_a6[i].getAttribute("src");
if(!src){
continue;
}
var m=src.match(_a7);
if(m){
var _ab=src.substring(0,m.index);
if(src.indexOf("bootstrap1")>-1){
_ab+="../";
}
if(!this["djConfig"]){
djConfig={};
}
if(djConfig["baseScriptUri"]==""){
djConfig["baseScriptUri"]=_ab;
}
if(djConfig["baseRelativePath"]==""){
djConfig["baseRelativePath"]=_ab;
}
break;
}
}
}
var dr=dojo.render;
var drh=dojo.render.html;
var drs=dojo.render.svg;
var dua=(drh.UA=navigator.userAgent);
var dav=(drh.AV=navigator.appVersion);
var t=true;
var f=false;
drh.capable=t;
drh.support.builtin=t;
dr.ver=parseFloat(drh.AV);
dr.os.mac=dav.indexOf("Macintosh")>=0;
dr.os.win=dav.indexOf("Windows")>=0;
dr.os.linux=dav.indexOf("X11")>=0;
drh.opera=dua.indexOf("Opera")>=0;
drh.khtml=(dav.indexOf("Konqueror")>=0)||(dav.indexOf("Safari")>=0);
drh.safari=dav.indexOf("Safari")>=0;
var _b3=dua.indexOf("Gecko");
drh.mozilla=drh.moz=(_b3>=0)&&(!drh.khtml);
if(drh.mozilla){
drh.geckoVersion=dua.substring(_b3+6,_b3+14);
}
drh.ie=(document.all)&&(!drh.opera);
drh.ie50=drh.ie&&dav.indexOf("MSIE 5.0")>=0;
drh.ie55=drh.ie&&dav.indexOf("MSIE 5.5")>=0;
drh.ie60=drh.ie&&dav.indexOf("MSIE 6.0")>=0;
drh.ie70=drh.ie&&dav.indexOf("MSIE 7.0")>=0;
var cm=document["compatMode"];
drh.quirks=(cm=="BackCompat")||(cm=="QuirksMode")||drh.ie55||drh.ie50;
dojo.locale=dojo.locale||(drh.ie?navigator.userLanguage:navigator.language).toLowerCase();
dr.vml.capable=drh.ie;
drs.capable=f;
drs.support.plugin=f;
drs.support.builtin=f;
var _b5=window["document"];
var tdi=_b5["implementation"];
if((tdi)&&(tdi["hasFeature"])&&(tdi.hasFeature("org.w3c.dom.svg","1.0"))){
drs.capable=t;
drs.support.builtin=t;
drs.support.plugin=f;
}
if(drh.safari){
var tmp=dua.split("AppleWebKit/")[1];
var ver=parseFloat(tmp.split(" ")[0]);
if(ver>=420){
drs.capable=t;
drs.support.builtin=t;
drs.support.plugin=f;
}
}else{
}
})();
dojo.hostenv.startPackage("dojo.hostenv");
dojo.render.name=dojo.hostenv.name_="browser";
dojo.hostenv.searchIds=[];
dojo.hostenv._XMLHTTP_PROGIDS=["Msxml2.XMLHTTP","Microsoft.XMLHTTP","Msxml2.XMLHTTP.4.0"];
dojo.hostenv.getXmlhttpObject=function(){
var _b9=null;
var _ba=null;
try{
_b9=new XMLHttpRequest();
}
catch(e){
}
if(!_b9){
for(var i=0;i<3;++i){
var _bc=dojo.hostenv._XMLHTTP_PROGIDS[i];
try{
_b9=new ActiveXObject(_bc);
}
catch(e){
_ba=e;
}
if(_b9){
dojo.hostenv._XMLHTTP_PROGIDS=[_bc];
break;
}
}
}
if(!_b9){
return dojo.raise("XMLHTTP not available",_ba);
}
return _b9;
};
dojo.hostenv._blockAsync=false;
dojo.hostenv.getText=function(uri,_be,_bf){
if(!_be){
this._blockAsync=true;
}
var _c0=this.getXmlhttpObject();
function isDocumentOk(_c1){
var _c2=_c1["status"];
return Boolean((!_c2)||((200<=_c2)&&(300>_c2))||(_c2==304));
}
if(_be){
var _c3=this,_c4=null,gbl=dojo.global();
var xhr=dojo.evalObjPath("dojo.io.XMLHTTPTransport");
_c0.onreadystatechange=function(){
if(_c4){
gbl.clearTimeout(_c4);
_c4=null;
}
if(_c3._blockAsync||(xhr&&xhr._blockAsync)){
_c4=gbl.setTimeout(function(){
_c0.onreadystatechange.apply(this);
},10);
}else{
if(4==_c0.readyState){
if(isDocumentOk(_c0)){
_be(_c0.responseText);
}
}
}
};
}
_c0.open("GET",uri,_be?true:false);
try{
_c0.send(null);
if(_be){
return null;
}
if(!isDocumentOk(_c0)){
var err=Error("Unable to load "+uri+" status:"+_c0.status);
err.status=_c0.status;
err.responseText=_c0.responseText;
throw err;
}
}
catch(e){
this._blockAsync=false;
if((_bf)&&(!_be)){
return null;
}else{
throw e;
}
}
this._blockAsync=false;
return _c0.responseText;
};
dojo.hostenv.defaultDebugContainerId="dojoDebug";
dojo.hostenv._println_buffer=[];
dojo.hostenv._println_safe=false;
dojo.hostenv.println=function(_c8){
if(!dojo.hostenv._println_safe){
dojo.hostenv._println_buffer.push(_c8);
}else{
try{
var _c9=document.getElementById(djConfig.debugContainerId?djConfig.debugContainerId:dojo.hostenv.defaultDebugContainerId);
if(!_c9){
_c9=dojo.body();
}
var div=document.createElement("div");
div.appendChild(document.createTextNode(_c8));
_c9.appendChild(div);
}
catch(e){
try{
document.write("<div>"+_c8+"</div>");
}
catch(e2){
window.status=_c8;
}
}
}
};
dojo.addOnLoad(function(){
dojo.hostenv._println_safe=true;
while(dojo.hostenv._println_buffer.length>0){
dojo.hostenv.println(dojo.hostenv._println_buffer.shift());
}
});
function dj_addNodeEvtHdlr(_cb,_cc,fp){
var _ce=_cb["on"+_cc]||function(){
};
_cb["on"+_cc]=function(){
fp.apply(_cb,arguments);
_ce.apply(_cb,arguments);
};
return true;
}
function dj_load_init(e){
var _d0=(e&&e.type)?e.type.toLowerCase():"load";
if(arguments.callee.initialized||(_d0!="domcontentloaded"&&_d0!="load")){
return;
}
arguments.callee.initialized=true;
if(typeof (_timer)!="undefined"){
clearInterval(_timer);
delete _timer;
}
var _d1=function(){
if(dojo.render.html.ie){
dojo.hostenv.makeWidgets();
}
};
if(dojo.hostenv.inFlightCount==0){
_d1();
dojo.hostenv.modulesLoaded();
}else{
dojo.hostenv.modulesLoadedListeners.unshift(_d1);
}
}
if(document.addEventListener){
if(dojo.render.html.opera||(dojo.render.html.moz&&(djConfig["enableMozDomContentLoaded"]===true))){
document.addEventListener("DOMContentLoaded",dj_load_init,null);
}
window.addEventListener("load",dj_load_init,null);
}
if(dojo.render.html.ie&&dojo.render.os.win){
document.attachEvent("onreadystatechange",function(e){
if(document.readyState=="complete"){
dj_load_init();
}
});
}
if(/(WebKit|khtml)/i.test(navigator.userAgent)){
var _timer=setInterval(function(){
if(/loaded|complete/.test(document.readyState)){
dj_load_init();
}
},10);
}
if(dojo.render.html.ie){
dj_addNodeEvtHdlr(window,"beforeunload",function(){
dojo.hostenv._unloading=true;
window.setTimeout(function(){
dojo.hostenv._unloading=false;
},0);
});
}
dj_addNodeEvtHdlr(window,"unload",function(){
dojo.hostenv.unloaded();
if((!dojo.render.html.ie)||(dojo.render.html.ie&&dojo.hostenv._unloading)){
dojo.hostenv.unloaded();
}
});
dojo.hostenv.makeWidgets=function(){
var _d3=[];
if(djConfig.searchIds&&djConfig.searchIds.length>0){
_d3=_d3.concat(djConfig.searchIds);
}
if(dojo.hostenv.searchIds&&dojo.hostenv.searchIds.length>0){
_d3=_d3.concat(dojo.hostenv.searchIds);
}
if((djConfig.parseWidgets)||(_d3.length>0)){
if(dojo.evalObjPath("dojo.widget.Parse")){
var _d4=new dojo.xml.Parse();
if(_d3.length>0){
for(var x=0;x<_d3.length;x++){
var _d6=document.getElementById(_d3[x]);
if(!_d6){
continue;
}
var _d7=_d4.parseElement(_d6,null,true);
dojo.widget.getParser().createComponents(_d7);
}
}else{
if(djConfig.parseWidgets){
var _d7=_d4.parseElement(dojo.body(),null,true);
dojo.widget.getParser().createComponents(_d7);
}
}
}
}
};
dojo.addOnLoad(function(){
if(!dojo.render.html.ie){
dojo.hostenv.makeWidgets();
}
});
try{
if(dojo.render.html.ie){
document.namespaces.add("v","urn:schemas-microsoft-com:vml");
document.createStyleSheet().addRule("v\\:*","behavior:url(#default#VML)");
}
}
catch(e){
}
dojo.hostenv.writeIncludes=function(){
};
if(!dj_undef("document",this)){
dj_currentDocument=this.document;
}
dojo.doc=function(){
return dj_currentDocument;
};
dojo.body=function(){
return dojo.doc().body||dojo.doc().getElementsByTagName("body")[0];
};
dojo.byId=function(id,doc){
if((id)&&((typeof id=="string")||(id instanceof String))){
if(!doc){
doc=dj_currentDocument;
}
var ele=doc.getElementById(id);
if(ele&&(ele.id!=id)&&doc.all){
ele=null;
eles=doc.all[id];
if(eles){
if(eles.length){
for(var i=0;i<eles.length;i++){
if(eles[i].id==id){
ele=eles[i];
break;
}
}
}else{
ele=eles;
}
}
}
return ele;
}
return id;
};
dojo.setContext=function(_dc,_dd){
dj_currentContext=_dc;
dj_currentDocument=_dd;
};
dojo._fireCallback=function(_de,_df,_e0){
if((_df)&&((typeof _de=="string")||(_de instanceof String))){
_de=_df[_de];
}
return (_df?_de.apply(_df,_e0||[]):_de());
};
dojo.withGlobal=function(_e1,_e2,_e3,_e4){
var _e5;
var _e6=dj_currentContext;
var _e7=dj_currentDocument;
try{
dojo.setContext(_e1,_e1.document);
_e5=dojo._fireCallback(_e2,_e3,_e4);
}
finally{
dojo.setContext(_e6,_e7);
}
return _e5;
};
dojo.withDoc=function(_e8,_e9,_ea,_eb){
var _ec;
var _ed=dj_currentDocument;
try{
dj_currentDocument=_e8;
_ec=dojo._fireCallback(_e9,_ea,_eb);
}
finally{
dj_currentDocument=_ed;
}
return _ec;
};
}
dojo.requireIf((djConfig["isDebug"]||djConfig["debugAtAllCosts"]),"dojo.debug");
dojo.requireIf(djConfig["debugAtAllCosts"]&&!window.widget&&!djConfig["useXDomain"],"dojo.browser_debug");
dojo.requireIf(djConfig["debugAtAllCosts"]&&!window.widget&&djConfig["useXDomain"],"dojo.browser_debug_xd");
dojo.provide("dojo.string.common");
dojo.string.trim=function(str,wh){
if(!str.replace){
return str;
}
if(!str.length){
return str;
}
var re=(wh>0)?(/^\s+/):(wh<0)?(/\s+$/):(/^\s+|\s+$/g);
return str.replace(re,"");
};
dojo.string.trimStart=function(str){
return dojo.string.trim(str,1);
};
dojo.string.trimEnd=function(str){
return dojo.string.trim(str,-1);
};
dojo.string.repeat=function(str,_f4,_f5){
var out="";
for(var i=0;i<_f4;i++){
out+=str;
if(_f5&&i<_f4-1){
out+=_f5;
}
}
return out;
};
dojo.string.pad=function(str,len,c,dir){
var out=String(str);
if(!c){
c="0";
}
if(!dir){
dir=1;
}
while(out.length<len){
if(dir>0){
out=c+out;
}else{
out+=c;
}
}
return out;
};
dojo.string.padLeft=function(str,len,c){
return dojo.string.pad(str,len,c,1);
};
dojo.string.padRight=function(str,len,c){
return dojo.string.pad(str,len,c,-1);
};
dojo.provide("dojo.string");
dojo.provide("dojo.lang.common");
dojo.lang.inherits=function(_103,_104){
if(!dojo.lang.isFunction(_104)){
dojo.raise("dojo.inherits: superclass argument ["+_104+"] must be a function (subclass: ["+_103+"']");
}
_103.prototype=new _104();
_103.prototype.constructor=_103;
_103.superclass=_104.prototype;
_103["super"]=_104.prototype;
};
dojo.lang._mixin=function(obj,_106){
var tobj={};
for(var x in _106){
if((typeof tobj[x]=="undefined")||(tobj[x]!=_106[x])){
obj[x]=_106[x];
}
}
if(dojo.render.html.ie&&(typeof (_106["toString"])=="function")&&(_106["toString"]!=obj["toString"])&&(_106["toString"]!=tobj["toString"])){
obj.toString=_106.toString;
}
return obj;
};
dojo.lang.mixin=function(obj,_10a){
for(var i=1,l=arguments.length;i<l;i++){
dojo.lang._mixin(obj,arguments[i]);
}
return obj;
};
dojo.lang.extend=function(_10d,_10e){
for(var i=1,l=arguments.length;i<l;i++){
dojo.lang._mixin(_10d.prototype,arguments[i]);
}
return _10d;
};
dojo.inherits=dojo.lang.inherits;
dojo.mixin=dojo.lang.mixin;
dojo.extend=dojo.lang.extend;
dojo.lang.find=function(_111,_112,_113,_114){
if(!dojo.lang.isArrayLike(_111)&&dojo.lang.isArrayLike(_112)){
dojo.deprecated("dojo.lang.find(value, array)","use dojo.lang.find(array, value) instead","0.5");
var temp=_111;
_111=_112;
_112=temp;
}
var _116=dojo.lang.isString(_111);
if(_116){
_111=_111.split("");
}
if(_114){
var step=-1;
var i=_111.length-1;
var end=-1;
}else{
var step=1;
var i=0;
var end=_111.length;
}
if(_113){
while(i!=end){
if(_111[i]===_112){
return i;
}
i+=step;
}
}else{
while(i!=end){
if(_111[i]==_112){
return i;
}
i+=step;
}
}
return -1;
};
dojo.lang.indexOf=dojo.lang.find;
dojo.lang.findLast=function(_11a,_11b,_11c){
return dojo.lang.find(_11a,_11b,_11c,true);
};
dojo.lang.lastIndexOf=dojo.lang.findLast;
dojo.lang.inArray=function(_11d,_11e){
return dojo.lang.find(_11d,_11e)>-1;
};
dojo.lang.isObject=function(it){
if(typeof it=="undefined"){
return false;
}
return (typeof it=="object"||it===null||dojo.lang.isArray(it)||dojo.lang.isFunction(it));
};
dojo.lang.isArray=function(it){
return (it&&it instanceof Array||typeof it=="array");
};
dojo.lang.isArrayLike=function(it){
if((!it)||(dojo.lang.isUndefined(it))){
return false;
}
if(dojo.lang.isString(it)){
return false;
}
if(dojo.lang.isFunction(it)){
return false;
}
if(dojo.lang.isArray(it)){
return true;
}
if((it.tagName)&&(it.tagName.toLowerCase()=="form")){
return false;
}
if(dojo.lang.isNumber(it.length)&&isFinite(it.length)){
return true;
}
return false;
};
dojo.lang.isFunction=function(it){
return (it instanceof Function||typeof it=="function");
};
(function(){
if((dojo.render.html.capable)&&(dojo.render.html["safari"])){
dojo.lang.isFunction=function(it){
if((typeof (it)=="function")&&(it=="[object NodeList]")){
return false;
}
return (it instanceof Function||typeof it=="function");
};
}
})();
dojo.lang.isString=function(it){
return (typeof it=="string"||it instanceof String);
};
dojo.lang.isAlien=function(it){
if(!it){
return false;
}
return !dojo.lang.isFunction(it)&&/\{\s*\[native code\]\s*\}/.test(String(it));
};
dojo.lang.isBoolean=function(it){
return (it instanceof Boolean||typeof it=="boolean");
};
dojo.lang.isNumber=function(it){
return (it instanceof Number||typeof it=="number");
};
dojo.lang.isUndefined=function(it){
return ((typeof (it)=="undefined")&&(it==undefined));
};
dojo.provide("dojo.lang.extras");
dojo.lang.setTimeout=function(func,_12a){
var _12b=window,_12c=2;
if(!dojo.lang.isFunction(func)){
_12b=func;
func=_12a;
_12a=arguments[2];
_12c++;
}
if(dojo.lang.isString(func)){
func=_12b[func];
}
var args=[];
for(var i=_12c;i<arguments.length;i++){
args.push(arguments[i]);
}
return dojo.global().setTimeout(function(){
func.apply(_12b,args);
},_12a);
};
dojo.lang.clearTimeout=function(_12f){
dojo.global().clearTimeout(_12f);
};
dojo.lang.getNameInObj=function(ns,item){
if(!ns){
ns=dj_global;
}
for(var x in ns){
if(ns[x]===item){
return new String(x);
}
}
return null;
};
dojo.lang.shallowCopy=function(obj,deep){
var i,ret;
if(obj===null){
return null;
}
if(dojo.lang.isObject(obj)){
ret=new obj.constructor();
for(i in obj){
if(dojo.lang.isUndefined(ret[i])){
ret[i]=deep?dojo.lang.shallowCopy(obj[i],deep):obj[i];
}
}
}else{
if(dojo.lang.isArray(obj)){
ret=[];
for(i=0;i<obj.length;i++){
ret[i]=deep?dojo.lang.shallowCopy(obj[i],deep):obj[i];
}
}else{
ret=obj;
}
}
return ret;
};
dojo.lang.firstValued=function(){
for(var i=0;i<arguments.length;i++){
if(typeof arguments[i]!="undefined"){
return arguments[i];
}
}
return undefined;
};
dojo.lang.getObjPathValue=function(_138,_139,_13a){
with(dojo.parseObjPath(_138,_139,_13a)){
return dojo.evalProp(prop,obj,_13a);
}
};
dojo.lang.setObjPathValue=function(_13b,_13c,_13d,_13e){
dojo.deprecated("dojo.lang.setObjPathValue","use dojo.parseObjPath and the '=' operator","0.6");
if(arguments.length<4){
_13e=true;
}
with(dojo.parseObjPath(_13b,_13d,_13e)){
if(obj&&(_13e||(prop in obj))){
obj[prop]=_13c;
}
}
};
dojo.provide("dojo.io.common");
dojo.io.transports=[];
dojo.io.hdlrFuncNames=["load","error","timeout"];
dojo.io.Request=function(url,_140,_141,_142){
if((arguments.length==1)&&(arguments[0].constructor==Object)){
this.fromKwArgs(arguments[0]);
}else{
this.url=url;
if(_140){
this.mimetype=_140;
}
if(_141){
this.transport=_141;
}
if(arguments.length>=4){
this.changeUrl=_142;
}
}
};
dojo.lang.extend(dojo.io.Request,{url:"",mimetype:"text/plain",method:"GET",content:undefined,transport:undefined,changeUrl:undefined,formNode:undefined,sync:false,bindSuccess:false,useCache:false,preventCache:false,load:function(type,data,_145,_146){
},error:function(type,_148,_149,_14a){
},timeout:function(type,_14c,_14d,_14e){
},handle:function(type,data,_151,_152){
},timeoutSeconds:0,abort:function(){
},fromKwArgs:function(_153){
if(_153["url"]){
_153.url=_153.url.toString();
}
if(_153["formNode"]){
_153.formNode=dojo.byId(_153.formNode);
}
if(!_153["method"]&&_153["formNode"]&&_153["formNode"].method){
_153.method=_153["formNode"].method;
}
if(!_153["handle"]&&_153["handler"]){
_153.handle=_153.handler;
}
if(!_153["load"]&&_153["loaded"]){
_153.load=_153.loaded;
}
if(!_153["changeUrl"]&&_153["changeURL"]){
_153.changeUrl=_153.changeURL;
}
_153.encoding=dojo.lang.firstValued(_153["encoding"],djConfig["bindEncoding"],"");
_153.sendTransport=dojo.lang.firstValued(_153["sendTransport"],djConfig["ioSendTransport"],false);
var _154=dojo.lang.isFunction;
for(var x=0;x<dojo.io.hdlrFuncNames.length;x++){
var fn=dojo.io.hdlrFuncNames[x];
if(_153[fn]&&_154(_153[fn])){
continue;
}
if(_153["handle"]&&_154(_153["handle"])){
_153[fn]=_153.handle;
}
}
dojo.lang.mixin(this,_153);
}});
dojo.io.Error=function(msg,type,num){
this.message=msg;
this.type=type||"unknown";
this.number=num||0;
};
dojo.io.transports.addTransport=function(name){
this.push(name);
this[name]=dojo.io[name];
};
dojo.io.bind=function(_15b){
if(!(_15b instanceof dojo.io.Request)){
try{
_15b=new dojo.io.Request(_15b);
}
catch(e){
dojo.debug(e);
}
}
var _15c="";
if(_15b["transport"]){
_15c=_15b["transport"];
if(!this[_15c]){
dojo.io.sendBindError(_15b,"No dojo.io.bind() transport with name '"+_15b["transport"]+"'.");
return _15b;
}
if(!this[_15c].canHandle(_15b)){
dojo.io.sendBindError(_15b,"dojo.io.bind() transport with name '"+_15b["transport"]+"' cannot handle this type of request.");
return _15b;
}
}else{
for(var x=0;x<dojo.io.transports.length;x++){
var tmp=dojo.io.transports[x];
if((this[tmp])&&(this[tmp].canHandle(_15b))){
_15c=tmp;
break;
}
}
if(_15c==""){
dojo.io.sendBindError(_15b,"None of the loaded transports for dojo.io.bind()"+" can handle the request.");
return _15b;
}
}
this[_15c].bind(_15b);
_15b.bindSuccess=true;
return _15b;
};
dojo.io.sendBindError=function(_15f,_160){
if((typeof _15f.error=="function"||typeof _15f.handle=="function")&&(typeof setTimeout=="function"||typeof setTimeout=="object")){
var _161=new dojo.io.Error(_160);
setTimeout(function(){
_15f[(typeof _15f.error=="function")?"error":"handle"]("error",_161,null,_15f);
},50);
}else{
dojo.raise(_160);
}
};
dojo.io.queueBind=function(_162){
if(!(_162 instanceof dojo.io.Request)){
try{
_162=new dojo.io.Request(_162);
}
catch(e){
dojo.debug(e);
}
}
var _163=_162.load;
_162.load=function(){
dojo.io._queueBindInFlight=false;
var ret=_163.apply(this,arguments);
dojo.io._dispatchNextQueueBind();
return ret;
};
var _165=_162.error;
_162.error=function(){
dojo.io._queueBindInFlight=false;
var ret=_165.apply(this,arguments);
dojo.io._dispatchNextQueueBind();
return ret;
};
dojo.io._bindQueue.push(_162);
dojo.io._dispatchNextQueueBind();
return _162;
};
dojo.io._dispatchNextQueueBind=function(){
if(!dojo.io._queueBindInFlight){
dojo.io._queueBindInFlight=true;
if(dojo.io._bindQueue.length>0){
dojo.io.bind(dojo.io._bindQueue.shift());
}else{
dojo.io._queueBindInFlight=false;
}
}
};
dojo.io._bindQueue=[];
dojo.io._queueBindInFlight=false;
dojo.io.argsFromMap=function(map,_168,last){
var enc=/utf/i.test(_168||"")?encodeURIComponent:dojo.string.encodeAscii;
var _16b=[];
var _16c=new Object();
for(var name in map){
var _16e=function(elt){
var val=enc(name)+"="+enc(elt);
_16b[(last==name)?"push":"unshift"](val);
};
if(!_16c[name]){
var _171=map[name];
if(dojo.lang.isArray(_171)){
dojo.lang.forEach(_171,_16e);
}else{
_16e(_171);
}
}
}
return _16b.join("&");
};
dojo.io.setIFrameSrc=function(_172,src,_174){
try{
var r=dojo.render.html;
if(!_174){
if(r.safari){
_172.location=src;
}else{
frames[_172.name].location=src;
}
}else{
var idoc;
if(r.ie){
idoc=_172.contentWindow.document;
}else{
if(r.safari){
idoc=_172.document;
}else{
idoc=_172.contentWindow;
}
}
if(!idoc){
_172.location=src;
return;
}else{
idoc.location.replace(src);
}
}
}
catch(e){
dojo.debug(e);
dojo.debug("setIFrameSrc: "+e);
}
};
dojo.provide("dojo.lang.array");
dojo.lang.mixin(dojo.lang,{has:function(obj,name){
try{
return typeof obj[name]!="undefined";
}
catch(e){
return false;
}
},isEmpty:function(obj){
if(dojo.lang.isObject(obj)){
var tmp={};
var _17b=0;
for(var x in obj){
if(obj[x]&&(!tmp[x])){
_17b++;
break;
}
}
return _17b==0;
}else{
if(dojo.lang.isArrayLike(obj)||dojo.lang.isString(obj)){
return obj.length==0;
}
}
},map:function(arr,obj,_17f){
var _180=dojo.lang.isString(arr);
if(_180){
arr=arr.split("");
}
if(dojo.lang.isFunction(obj)&&(!_17f)){
_17f=obj;
obj=dj_global;
}else{
if(dojo.lang.isFunction(obj)&&_17f){
var _181=obj;
obj=_17f;
_17f=_181;
}
}
if(Array.map){
var _182=Array.map(arr,_17f,obj);
}else{
var _182=[];
for(var i=0;i<arr.length;++i){
_182.push(_17f.call(obj,arr[i]));
}
}
if(_180){
return _182.join("");
}else{
return _182;
}
},reduce:function(arr,_185,obj,_187){
var _188=_185;
if(arguments.length==2){
_187=_185;
_188=arr[0];
arr=arr.slice(1);
}else{
if(arguments.length==3){
if(dojo.lang.isFunction(obj)){
_187=obj;
obj=null;
}
}else{
if(dojo.lang.isFunction(obj)){
var tmp=_187;
_187=obj;
obj=tmp;
}
}
}
var ob=obj||dj_global;
dojo.lang.map(arr,function(val){
_188=_187.call(ob,_188,val);
});
return _188;
},forEach:function(_18c,_18d,_18e){
if(dojo.lang.isString(_18c)){
_18c=_18c.split("");
}
if(Array.forEach){
Array.forEach(_18c,_18d,_18e);
}else{
if(!_18e){
_18e=dj_global;
}
for(var i=0,l=_18c.length;i<l;i++){
_18d.call(_18e,_18c[i],i,_18c);
}
}
},_everyOrSome:function(_191,arr,_193,_194){
if(dojo.lang.isString(arr)){
arr=arr.split("");
}
if(Array.every){
return Array[_191?"every":"some"](arr,_193,_194);
}else{
if(!_194){
_194=dj_global;
}
for(var i=0,l=arr.length;i<l;i++){
var _197=_193.call(_194,arr[i],i,arr);
if(_191&&!_197){
return false;
}else{
if((!_191)&&(_197)){
return true;
}
}
}
return Boolean(_191);
}
},every:function(arr,_199,_19a){
return this._everyOrSome(true,arr,_199,_19a);
},some:function(arr,_19c,_19d){
return this._everyOrSome(false,arr,_19c,_19d);
},filter:function(arr,_19f,_1a0){
var _1a1=dojo.lang.isString(arr);
if(_1a1){
arr=arr.split("");
}
var _1a2;
if(Array.filter){
_1a2=Array.filter(arr,_19f,_1a0);
}else{
if(!_1a0){
if(arguments.length>=3){
dojo.raise("thisObject doesn't exist!");
}
_1a0=dj_global;
}
_1a2=[];
for(var i=0;i<arr.length;i++){
if(_19f.call(_1a0,arr[i],i,arr)){
_1a2.push(arr[i]);
}
}
}
if(_1a1){
return _1a2.join("");
}else{
return _1a2;
}
},unnest:function(){
var out=[];
for(var i=0;i<arguments.length;i++){
if(dojo.lang.isArrayLike(arguments[i])){
var add=dojo.lang.unnest.apply(this,arguments[i]);
out=out.concat(add);
}else{
out.push(arguments[i]);
}
}
return out;
},toArray:function(_1a7,_1a8){
var _1a9=[];
for(var i=_1a8||0;i<_1a7.length;i++){
_1a9.push(_1a7[i]);
}
return _1a9;
}});
dojo.provide("dojo.lang.func");
dojo.lang.hitch=function(_1ab,_1ac){
var fcn=(dojo.lang.isString(_1ac)?_1ab[_1ac]:_1ac)||function(){
};
return function(){
return fcn.apply(_1ab,arguments);
};
};
dojo.lang.anonCtr=0;
dojo.lang.anon={};
dojo.lang.nameAnonFunc=function(_1ae,_1af,_1b0){
var nso=(_1af||dojo.lang.anon);
if((_1b0)||((dj_global["djConfig"])&&(djConfig["slowAnonFuncLookups"]==true))){
for(var x in nso){
try{
if(nso[x]===_1ae){
return x;
}
}
catch(e){
}
}
}
var ret="__"+dojo.lang.anonCtr++;
while(typeof nso[ret]!="undefined"){
ret="__"+dojo.lang.anonCtr++;
}
nso[ret]=_1ae;
return ret;
};
dojo.lang.forward=function(_1b4){
return function(){
return this[_1b4].apply(this,arguments);
};
};
dojo.lang.curry=function(_1b5,func){
var _1b7=[];
_1b5=_1b5||dj_global;
if(dojo.lang.isString(func)){
func=_1b5[func];
}
for(var x=2;x<arguments.length;x++){
_1b7.push(arguments[x]);
}
var _1b9=(func["__preJoinArity"]||func.length)-_1b7.length;
function gather(_1ba,_1bb,_1bc){
var _1bd=_1bc;
var _1be=_1bb.slice(0);
for(var x=0;x<_1ba.length;x++){
_1be.push(_1ba[x]);
}
_1bc=_1bc-_1ba.length;
if(_1bc<=0){
var res=func.apply(_1b5,_1be);
_1bc=_1bd;
return res;
}else{
return function(){
return gather(arguments,_1be,_1bc);
};
}
}
return gather([],_1b7,_1b9);
};
dojo.lang.curryArguments=function(_1c1,func,args,_1c4){
var _1c5=[];
var x=_1c4||0;
for(x=_1c4;x<args.length;x++){
_1c5.push(args[x]);
}
return dojo.lang.curry.apply(dojo.lang,[_1c1,func].concat(_1c5));
};
dojo.lang.tryThese=function(){
for(var x=0;x<arguments.length;x++){
try{
if(typeof arguments[x]=="function"){
var ret=(arguments[x]());
if(ret){
return ret;
}
}
}
catch(e){
dojo.debug(e);
}
}
};
dojo.lang.delayThese=function(farr,cb,_1cb,_1cc){
if(!farr.length){
if(typeof _1cc=="function"){
_1cc();
}
return;
}
if((typeof _1cb=="undefined")&&(typeof cb=="number")){
_1cb=cb;
cb=function(){
};
}else{
if(!cb){
cb=function(){
};
if(!_1cb){
_1cb=0;
}
}
}
setTimeout(function(){
(farr.shift())();
cb();
dojo.lang.delayThese(farr,cb,_1cb,_1cc);
},_1cb);
};
dojo.provide("dojo.string.extras");
dojo.string.substituteParams=function(_1cd,hash){
var map=(typeof hash=="object")?hash:dojo.lang.toArray(arguments,1);
return _1cd.replace(/\%\{(\w+)\}/g,function(_1d0,key){
if(typeof (map[key])!="undefined"&&map[key]!=null){
return map[key];
}
dojo.raise("Substitution not found: "+key);
});
};
dojo.string.capitalize=function(str){
if(!dojo.lang.isString(str)){
return "";
}
if(arguments.length==0){
str=this;
}
var _1d3=str.split(" ");
for(var i=0;i<_1d3.length;i++){
_1d3[i]=_1d3[i].charAt(0).toUpperCase()+_1d3[i].substring(1);
}
return _1d3.join(" ");
};
dojo.string.isBlank=function(str){
if(!dojo.lang.isString(str)){
return true;
}
return (dojo.string.trim(str).length==0);
};
dojo.string.encodeAscii=function(str){
if(!dojo.lang.isString(str)){
return str;
}
var ret="";
var _1d8=escape(str);
var _1d9,re=/%u([0-9A-F]{4})/i;
while((_1d9=_1d8.match(re))){
var num=Number("0x"+_1d9[1]);
var _1dc=escape("&#"+num+";");
ret+=_1d8.substring(0,_1d9.index)+_1dc;
_1d8=_1d8.substring(_1d9.index+_1d9[0].length);
}
ret+=_1d8.replace(/\+/g,"%2B");
return ret;
};
dojo.string.escape=function(type,str){
var args=dojo.lang.toArray(arguments,1);
switch(type.toLowerCase()){
case "xml":
case "html":
case "xhtml":
return dojo.string.escapeXml.apply(this,args);
case "sql":
return dojo.string.escapeSql.apply(this,args);
case "regexp":
case "regex":
return dojo.string.escapeRegExp.apply(this,args);
case "javascript":
case "jscript":
case "js":
return dojo.string.escapeJavaScript.apply(this,args);
case "ascii":
return dojo.string.encodeAscii.apply(this,args);
default:
return str;
}
};
dojo.string.escapeXml=function(str,_1e1){
str=str.replace(/&/gm,"&amp;").replace(/</gm,"&lt;").replace(/>/gm,"&gt;").replace(/"/gm,"&quot;");
if(!_1e1){
str=str.replace(/'/gm,"&#39;");
}
return str;
};
dojo.string.escapeSql=function(str){
return str.replace(/'/gm,"''");
};
dojo.string.escapeRegExp=function(str){
return str.replace(/\\/gm,"\\\\").replace(/([\f\b\n\t\r[\^$|?*+(){}])/gm,"\\$1");
};
dojo.string.escapeJavaScript=function(str){
return str.replace(/(["'\f\b\n\t\r])/gm,"\\$1");
};
dojo.string.escapeString=function(str){
return ("\""+str.replace(/(["\\])/g,"\\$1")+"\"").replace(/[\f]/g,"\\f").replace(/[\b]/g,"\\b").replace(/[\n]/g,"\\n").replace(/[\t]/g,"\\t").replace(/[\r]/g,"\\r");
};
dojo.string.summary=function(str,len){
if(!len||str.length<=len){
return str;
}
return str.substring(0,len).replace(/\.+$/,"")+"...";
};
dojo.string.endsWith=function(str,end,_1ea){
if(_1ea){
str=str.toLowerCase();
end=end.toLowerCase();
}
if((str.length-end.length)<0){
return false;
}
return str.lastIndexOf(end)==str.length-end.length;
};
dojo.string.endsWithAny=function(str){
for(var i=1;i<arguments.length;i++){
if(dojo.string.endsWith(str,arguments[i])){
return true;
}
}
return false;
};
dojo.string.startsWith=function(str,_1ee,_1ef){
if(_1ef){
str=str.toLowerCase();
_1ee=_1ee.toLowerCase();
}
return str.indexOf(_1ee)==0;
};
dojo.string.startsWithAny=function(str){
for(var i=1;i<arguments.length;i++){
if(dojo.string.startsWith(str,arguments[i])){
return true;
}
}
return false;
};
dojo.string.has=function(str){
for(var i=1;i<arguments.length;i++){
if(str.indexOf(arguments[i])>-1){
return true;
}
}
return false;
};
dojo.string.normalizeNewlines=function(text,_1f5){
if(_1f5=="\n"){
text=text.replace(/\r\n/g,"\n");
text=text.replace(/\r/g,"\n");
}else{
if(_1f5=="\r"){
text=text.replace(/\r\n/g,"\r");
text=text.replace(/\n/g,"\r");
}else{
text=text.replace(/([^\r])\n/g,"$1\r\n").replace(/\r([^\n])/g,"\r\n$1");
}
}
return text;
};
dojo.string.splitEscaped=function(str,_1f7){
var _1f8=[];
for(var i=0,_1fa=0;i<str.length;i++){
if(str.charAt(i)=="\\"){
i++;
continue;
}
if(str.charAt(i)==_1f7){
_1f8.push(str.substring(_1fa,i));
_1fa=i+1;
}
}
_1f8.push(str.substr(_1fa));
return _1f8;
};
dojo.provide("dojo.dom");
dojo.dom.ELEMENT_NODE=1;
dojo.dom.ATTRIBUTE_NODE=2;
dojo.dom.TEXT_NODE=3;
dojo.dom.CDATA_SECTION_NODE=4;
dojo.dom.ENTITY_REFERENCE_NODE=5;
dojo.dom.ENTITY_NODE=6;
dojo.dom.PROCESSING_INSTRUCTION_NODE=7;
dojo.dom.COMMENT_NODE=8;
dojo.dom.DOCUMENT_NODE=9;
dojo.dom.DOCUMENT_TYPE_NODE=10;
dojo.dom.DOCUMENT_FRAGMENT_NODE=11;
dojo.dom.NOTATION_NODE=12;
dojo.dom.dojoml="http://www.dojotoolkit.org/2004/dojoml";
dojo.dom.xmlns={svg:"http://www.w3.org/2000/svg",smil:"http://www.w3.org/2001/SMIL20/",mml:"http://www.w3.org/1998/Math/MathML",cml:"http://www.xml-cml.org",xlink:"http://www.w3.org/1999/xlink",xhtml:"http://www.w3.org/1999/xhtml",xul:"http://www.mozilla.org/keymaster/gatekeeper/there.is.only.xul",xbl:"http://www.mozilla.org/xbl",fo:"http://www.w3.org/1999/XSL/Format",xsl:"http://www.w3.org/1999/XSL/Transform",xslt:"http://www.w3.org/1999/XSL/Transform",xi:"http://www.w3.org/2001/XInclude",xforms:"http://www.w3.org/2002/01/xforms",saxon:"http://icl.com/saxon",xalan:"http://xml.apache.org/xslt",xsd:"http://www.w3.org/2001/XMLSchema",dt:"http://www.w3.org/2001/XMLSchema-datatypes",xsi:"http://www.w3.org/2001/XMLSchema-instance",rdf:"http://www.w3.org/1999/02/22-rdf-syntax-ns#",rdfs:"http://www.w3.org/2000/01/rdf-schema#",dc:"http://purl.org/dc/elements/1.1/",dcq:"http://purl.org/dc/qualifiers/1.0","soap-env":"http://schemas.xmlsoap.org/soap/envelope/",wsdl:"http://schemas.xmlsoap.org/wsdl/",AdobeExtensions:"http://ns.adobe.com/AdobeSVGViewerExtensions/3.0/"};
dojo.dom.isNode=function(wh){
if(typeof Element=="function"){
try{
return wh instanceof Element;
}
catch(e){
}
}else{
return wh&&!isNaN(wh.nodeType);
}
};
dojo.dom.getUniqueId=function(){
var _1fc=dojo.doc();
do{
var id="dj_unique_"+(++arguments.callee._idIncrement);
}while(_1fc.getElementById(id));
return id;
};
dojo.dom.getUniqueId._idIncrement=0;
dojo.dom.firstElement=dojo.dom.getFirstChildElement=function(_1fe,_1ff){
var node=_1fe.firstChild;
while(node&&node.nodeType!=dojo.dom.ELEMENT_NODE){
node=node.nextSibling;
}
if(_1ff&&node&&node.tagName&&node.tagName.toLowerCase()!=_1ff.toLowerCase()){
node=dojo.dom.nextElement(node,_1ff);
}
return node;
};
dojo.dom.lastElement=dojo.dom.getLastChildElement=function(_201,_202){
var node=_201.lastChild;
while(node&&node.nodeType!=dojo.dom.ELEMENT_NODE){
node=node.previousSibling;
}
if(_202&&node&&node.tagName&&node.tagName.toLowerCase()!=_202.toLowerCase()){
node=dojo.dom.prevElement(node,_202);
}
return node;
};
dojo.dom.nextElement=dojo.dom.getNextSiblingElement=function(node,_205){
if(!node){
return null;
}
do{
node=node.nextSibling;
}while(node&&node.nodeType!=dojo.dom.ELEMENT_NODE);
if(node&&_205&&_205.toLowerCase()!=node.tagName.toLowerCase()){
return dojo.dom.nextElement(node,_205);
}
return node;
};
dojo.dom.prevElement=dojo.dom.getPreviousSiblingElement=function(node,_207){
if(!node){
return null;
}
if(_207){
_207=_207.toLowerCase();
}
do{
node=node.previousSibling;
}while(node&&node.nodeType!=dojo.dom.ELEMENT_NODE);
if(node&&_207&&_207.toLowerCase()!=node.tagName.toLowerCase()){
return dojo.dom.prevElement(node,_207);
}
return node;
};
dojo.dom.moveChildren=function(_208,_209,trim){
var _20b=0;
if(trim){
while(_208.hasChildNodes()&&_208.firstChild.nodeType==dojo.dom.TEXT_NODE){
_208.removeChild(_208.firstChild);
}
while(_208.hasChildNodes()&&_208.lastChild.nodeType==dojo.dom.TEXT_NODE){
_208.removeChild(_208.lastChild);
}
}
while(_208.hasChildNodes()){
_209.appendChild(_208.firstChild);
_20b++;
}
return _20b;
};
dojo.dom.copyChildren=function(_20c,_20d,trim){
var _20f=_20c.cloneNode(true);
return this.moveChildren(_20f,_20d,trim);
};
dojo.dom.replaceChildren=function(node,_211){
var _212=[];
if(dojo.render.html.ie){
for(var i=0;i<node.childNodes.length;i++){
_212.push(node.childNodes[i]);
}
}
dojo.dom.removeChildren(node);
node.appendChild(_211);
for(var i=0;i<_212.length;i++){
dojo.dom.destroyNode(_212[i]);
}
};
dojo.dom.removeChildren=function(node){
var _215=node.childNodes.length;
while(node.hasChildNodes()){
dojo.dom.removeNode(node.firstChild);
}
return _215;
};
dojo.dom.replaceNode=function(node,_217){
return node.parentNode.replaceChild(_217,node);
};
dojo.dom.destroyNode=function(node){
if(node.parentNode){
node=dojo.dom.removeNode(node);
}
if(node.nodeType!=3){
if(dojo.evalObjPath("dojo.event.browser.clean",false)){
dojo.event.browser.clean(node);
}
if(dojo.render.html.ie){
node.outerHTML="";
}
}
};
dojo.dom.removeNode=function(node){
if(node&&node.parentNode){
return node.parentNode.removeChild(node);
}
};
dojo.dom.getAncestors=function(node,_21b,_21c){
var _21d=[];
var _21e=(_21b&&(_21b instanceof Function||typeof _21b=="function"));
while(node){
if(!_21e||_21b(node)){
_21d.push(node);
}
if(_21c&&_21d.length>0){
return _21d[0];
}
node=node.parentNode;
}
if(_21c){
return null;
}
return _21d;
};
dojo.dom.getAncestorsByTag=function(node,tag,_221){
tag=tag.toLowerCase();
return dojo.dom.getAncestors(node,function(el){
return ((el.tagName)&&(el.tagName.toLowerCase()==tag));
},_221);
};
dojo.dom.getFirstAncestorByTag=function(node,tag){
return dojo.dom.getAncestorsByTag(node,tag,true);
};
dojo.dom.isDescendantOf=function(node,_226,_227){
if(_227&&node){
node=node.parentNode;
}
while(node){
if(node==_226){
return true;
}
node=node.parentNode;
}
return false;
};
dojo.dom.innerXML=function(node){
if(node.innerXML){
return node.innerXML;
}else{
if(node.xml){
return node.xml;
}else{
if(typeof XMLSerializer!="undefined"){
return (new XMLSerializer()).serializeToString(node);
}
}
}
};
dojo.dom.createDocument=function(){
var doc=null;
var _22a=dojo.doc();
if(!dj_undef("ActiveXObject")){
var _22b=["MSXML2","Microsoft","MSXML","MSXML3"];
for(var i=0;i<_22b.length;i++){
try{
doc=new ActiveXObject(_22b[i]+".XMLDOM");
}
catch(e){
}
if(doc){
break;
}
}
}else{
if((_22a.implementation)&&(_22a.implementation.createDocument)){
doc=_22a.implementation.createDocument("","",null);
}
}
return doc;
};
dojo.dom.createDocumentFromText=function(str,_22e){
if(!_22e){
_22e="text/xml";
}
if(!dj_undef("DOMParser")){
var _22f=new DOMParser();
return _22f.parseFromString(str,_22e);
}else{
if(!dj_undef("ActiveXObject")){
var _230=dojo.dom.createDocument();
if(_230){
_230.async=false;
_230.loadXML(str);
return _230;
}else{
dojo.debug("toXml didn't work?");
}
}else{
var _231=dojo.doc();
if(_231.createElement){
var tmp=_231.createElement("xml");
tmp.innerHTML=str;
if(_231.implementation&&_231.implementation.createDocument){
var _233=_231.implementation.createDocument("foo","",null);
for(var i=0;i<tmp.childNodes.length;i++){
_233.importNode(tmp.childNodes.item(i),true);
}
return _233;
}
return ((tmp.document)&&(tmp.document.firstChild?tmp.document.firstChild:tmp));
}
}
}
return null;
};
dojo.dom.prependChild=function(node,_236){
if(_236.firstChild){
_236.insertBefore(node,_236.firstChild);
}else{
_236.appendChild(node);
}
return true;
};
dojo.dom.insertBefore=function(node,ref,_239){
if((_239!=true)&&(node===ref||node.nextSibling===ref)){
return false;
}
var _23a=ref.parentNode;
_23a.insertBefore(node,ref);
return true;
};
dojo.dom.insertAfter=function(node,ref,_23d){
var pn=ref.parentNode;
if(ref==pn.lastChild){
if((_23d!=true)&&(node===ref)){
return false;
}
pn.appendChild(node);
}else{
return this.insertBefore(node,ref.nextSibling,_23d);
}
return true;
};
dojo.dom.insertAtPosition=function(node,ref,_241){
if((!node)||(!ref)||(!_241)){
return false;
}
switch(_241.toLowerCase()){
case "before":
return dojo.dom.insertBefore(node,ref);
case "after":
return dojo.dom.insertAfter(node,ref);
case "first":
if(ref.firstChild){
return dojo.dom.insertBefore(node,ref.firstChild);
}else{
ref.appendChild(node);
return true;
}
break;
default:
ref.appendChild(node);
return true;
}
};
dojo.dom.insertAtIndex=function(node,_243,_244){
var _245=_243.childNodes;
if(!_245.length||_245.length==_244){
_243.appendChild(node);
return true;
}
if(_244==0){
return dojo.dom.prependChild(node,_243);
}
return dojo.dom.insertAfter(node,_245[_244-1]);
};
dojo.dom.textContent=function(node,text){
if(arguments.length>1){
var _248=dojo.doc();
dojo.dom.replaceChildren(node,_248.createTextNode(text));
return text;
}else{
if(node.textContent!=undefined){
return node.textContent;
}
var _249="";
if(node==null){
return _249;
}
for(var i=0;i<node.childNodes.length;i++){
switch(node.childNodes[i].nodeType){
case 1:
case 5:
_249+=dojo.dom.textContent(node.childNodes[i]);
break;
case 3:
case 2:
case 4:
_249+=node.childNodes[i].nodeValue;
break;
default:
break;
}
}
return _249;
}
};
dojo.dom.hasParent=function(node){
return Boolean(node&&node.parentNode&&dojo.dom.isNode(node.parentNode));
};
dojo.dom.isTag=function(node){
if(node&&node.tagName){
for(var i=1;i<arguments.length;i++){
if(node.tagName==String(arguments[i])){
return String(arguments[i]);
}
}
}
return "";
};
dojo.dom.setAttributeNS=function(elem,_24f,_250,_251){
if(elem==null||((elem==undefined)&&(typeof elem=="undefined"))){
dojo.raise("No element given to dojo.dom.setAttributeNS");
}
if(!((elem.setAttributeNS==undefined)&&(typeof elem.setAttributeNS=="undefined"))){
elem.setAttributeNS(_24f,_250,_251);
}else{
var _252=elem.ownerDocument;
var _253=_252.createNode(2,_250,_24f);
_253.nodeValue=_251;
elem.setAttributeNode(_253);
}
};
dojo.provide("dojo.undo.browser");
try{
if((!djConfig["preventBackButtonFix"])&&(!dojo.hostenv.post_load_)){
document.write("<iframe style='border: 0px; width: 1px; height: 1px; position: absolute; bottom: 0px; right: 0px; visibility: visible;' name='djhistory' id='djhistory' src='"+(djConfig["dojoIframeHistoryUrl"]||dojo.hostenv.getBaseScriptUri()+"iframe_history.html")+"'></iframe>");
}
}
catch(e){
}
if(dojo.render.html.opera){
dojo.debug("Opera is not supported with dojo.undo.browser, so back/forward detection will not work.");
}
dojo.undo.browser={initialHref:(!dj_undef("window"))?window.location.href:"",initialHash:(!dj_undef("window"))?window.location.hash:"",moveForward:false,historyStack:[],forwardStack:[],historyIframe:null,bookmarkAnchor:null,locationTimer:null,setInitialState:function(args){
this.initialState=this._createState(this.initialHref,args,this.initialHash);
},addToHistory:function(args){
this.forwardStack=[];
var hash=null;
var url=null;
if(!this.historyIframe){
if(djConfig["useXDomain"]&&!djConfig["dojoIframeHistoryUrl"]){
dojo.debug("dojo.undo.browser: When using cross-domain Dojo builds,"+" please save iframe_history.html to your domain and set djConfig.dojoIframeHistoryUrl"+" to the path on your domain to iframe_history.html");
}
this.historyIframe=window.frames["djhistory"];
}
if(!this.bookmarkAnchor){
this.bookmarkAnchor=document.createElement("a");
dojo.body().appendChild(this.bookmarkAnchor);
this.bookmarkAnchor.style.display="none";
}
if(args["changeUrl"]){
hash="#"+((args["changeUrl"]!==true)?args["changeUrl"]:(new Date()).getTime());
if(this.historyStack.length==0&&this.initialState.urlHash==hash){
this.initialState=this._createState(url,args,hash);
return;
}else{
if(this.historyStack.length>0&&this.historyStack[this.historyStack.length-1].urlHash==hash){
this.historyStack[this.historyStack.length-1]=this._createState(url,args,hash);
return;
}
}
this.changingUrl=true;
setTimeout("window.location.href = '"+hash+"'; dojo.undo.browser.changingUrl = false;",1);
this.bookmarkAnchor.href=hash;
if(dojo.render.html.ie){
url=this._loadIframeHistory();
var _258=args["back"]||args["backButton"]||args["handle"];
var tcb=function(_25a){
if(window.location.hash!=""){
setTimeout("window.location.href = '"+hash+"';",1);
}
_258.apply(this,[_25a]);
};
if(args["back"]){
args.back=tcb;
}else{
if(args["backButton"]){
args.backButton=tcb;
}else{
if(args["handle"]){
args.handle=tcb;
}
}
}
var _25b=args["forward"]||args["forwardButton"]||args["handle"];
var tfw=function(_25d){
if(window.location.hash!=""){
window.location.href=hash;
}
if(_25b){
_25b.apply(this,[_25d]);
}
};
if(args["forward"]){
args.forward=tfw;
}else{
if(args["forwardButton"]){
args.forwardButton=tfw;
}else{
if(args["handle"]){
args.handle=tfw;
}
}
}
}else{
if(dojo.render.html.moz){
if(!this.locationTimer){
this.locationTimer=setInterval("dojo.undo.browser.checkLocation();",200);
}
}
}
}else{
url=this._loadIframeHistory();
}
this.historyStack.push(this._createState(url,args,hash));
},checkLocation:function(){
if(!this.changingUrl){
var hsl=this.historyStack.length;
if((window.location.hash==this.initialHash||window.location.href==this.initialHref)&&(hsl==1)){
this.handleBackButton();
return;
}
if(this.forwardStack.length>0){
if(this.forwardStack[this.forwardStack.length-1].urlHash==window.location.hash){
this.handleForwardButton();
return;
}
}
if((hsl>=2)&&(this.historyStack[hsl-2])){
if(this.historyStack[hsl-2].urlHash==window.location.hash){
this.handleBackButton();
return;
}
}
}
},iframeLoaded:function(evt,_260){
if(!dojo.render.html.opera){
var _261=this._getUrlQuery(_260.href);
if(_261==null){
if(this.historyStack.length==1){
this.handleBackButton();
}
return;
}
if(this.moveForward){
this.moveForward=false;
return;
}
if(this.historyStack.length>=2&&_261==this._getUrlQuery(this.historyStack[this.historyStack.length-2].url)){
this.handleBackButton();
}else{
if(this.forwardStack.length>0&&_261==this._getUrlQuery(this.forwardStack[this.forwardStack.length-1].url)){
this.handleForwardButton();
}
}
}
},handleBackButton:function(){
var _262=this.historyStack.pop();
if(!_262){
return;
}
var last=this.historyStack[this.historyStack.length-1];
if(!last&&this.historyStack.length==0){
last=this.initialState;
}
if(last){
if(last.kwArgs["back"]){
last.kwArgs["back"]();
}else{
if(last.kwArgs["backButton"]){
last.kwArgs["backButton"]();
}else{
if(last.kwArgs["handle"]){
last.kwArgs.handle("back");
}
}
}
}
this.forwardStack.push(_262);
},handleForwardButton:function(){
var last=this.forwardStack.pop();
if(!last){
return;
}
if(last.kwArgs["forward"]){
last.kwArgs.forward();
}else{
if(last.kwArgs["forwardButton"]){
last.kwArgs.forwardButton();
}else{
if(last.kwArgs["handle"]){
last.kwArgs.handle("forward");
}
}
}
this.historyStack.push(last);
},_createState:function(url,args,hash){
return {"url":url,"kwArgs":args,"urlHash":hash};
},_getUrlQuery:function(url){
var _269=url.split("?");
if(_269.length<2){
return null;
}else{
return _269[1];
}
},_loadIframeHistory:function(){
var url=(djConfig["dojoIframeHistoryUrl"]||dojo.hostenv.getBaseScriptUri()+"iframe_history.html")+"?"+(new Date()).getTime();
this.moveForward=true;
dojo.io.setIFrameSrc(this.historyIframe,url,false);
return url;
}};
dojo.provide("dojo.io.BrowserIO");
if(!dj_undef("window")){
dojo.io.checkChildrenForFile=function(node){
var _26c=false;
var _26d=node.getElementsByTagName("input");
dojo.lang.forEach(_26d,function(_26e){
if(_26c){
return;
}
if(_26e.getAttribute("type")=="file"){
_26c=true;
}
});
return _26c;
};
dojo.io.formHasFile=function(_26f){
return dojo.io.checkChildrenForFile(_26f);
};
dojo.io.updateNode=function(node,_271){
node=dojo.byId(node);
var args=_271;
if(dojo.lang.isString(_271)){
args={url:_271};
}
args.mimetype="text/html";
args.load=function(t,d,e){
while(node.firstChild){
dojo.dom.destroyNode(node.firstChild);
}
node.innerHTML=d;
};
dojo.io.bind(args);
};
dojo.io.formFilter=function(node){
var type=(node.type||"").toLowerCase();
return !node.disabled&&node.name&&!dojo.lang.inArray(["file","submit","image","reset","button"],type);
};
dojo.io.encodeForm=function(_278,_279,_27a){
if((!_278)||(!_278.tagName)||(!_278.tagName.toLowerCase()=="form")){
dojo.raise("Attempted to encode a non-form element.");
}
if(!_27a){
_27a=dojo.io.formFilter;
}
var enc=/utf/i.test(_279||"")?encodeURIComponent:dojo.string.encodeAscii;
var _27c=[];
for(var i=0;i<_278.elements.length;i++){
var elm=_278.elements[i];
if(!elm||elm.tagName.toLowerCase()=="fieldset"||!_27a(elm)){
continue;
}
var name=enc(elm.name);
var type=elm.type.toLowerCase();
if(type=="select-multiple"){
for(var j=0;j<elm.options.length;j++){
if(elm.options[j].selected){
_27c.push(name+"="+enc(elm.options[j].value));
}
}
}else{
if(dojo.lang.inArray(["radio","checkbox"],type)){
if(elm.checked){
_27c.push(name+"="+enc(elm.value));
}
}else{
_27c.push(name+"="+enc(elm.value));
}
}
}
var _282=_278.getElementsByTagName("input");
for(var i=0;i<_282.length;i++){
var _283=_282[i];
if(_283.type.toLowerCase()=="image"&&_283.form==_278&&_27a(_283)){
var name=enc(_283.name);
_27c.push(name+"="+enc(_283.value));
_27c.push(name+".x=0");
_27c.push(name+".y=0");
}
}
return _27c.join("&")+"&";
};
dojo.io.FormBind=function(args){
this.bindArgs={};
if(args&&args.formNode){
this.init(args);
}else{
if(args){
this.init({formNode:args});
}
}
};
dojo.lang.extend(dojo.io.FormBind,{form:null,bindArgs:null,clickedButton:null,init:function(args){
var form=dojo.byId(args.formNode);
if(!form||!form.tagName||form.tagName.toLowerCase()!="form"){
throw new Error("FormBind: Couldn't apply, invalid form");
}else{
if(this.form==form){
return;
}else{
if(this.form){
throw new Error("FormBind: Already applied to a form");
}
}
}
dojo.lang.mixin(this.bindArgs,args);
this.form=form;
this.connect(form,"onsubmit","submit");
for(var i=0;i<form.elements.length;i++){
var node=form.elements[i];
if(node&&node.type&&dojo.lang.inArray(["submit","button"],node.type.toLowerCase())){
this.connect(node,"onclick","click");
}
}
var _289=form.getElementsByTagName("input");
for(var i=0;i<_289.length;i++){
var _28a=_289[i];
if(_28a.type.toLowerCase()=="image"&&_28a.form==form){
this.connect(_28a,"onclick","click");
}
}
},onSubmit:function(form){
return true;
},submit:function(e){
e.preventDefault();
if(this.onSubmit(this.form)){
dojo.io.bind(dojo.lang.mixin(this.bindArgs,{formFilter:dojo.lang.hitch(this,"formFilter")}));
}
},click:function(e){
var node=e.currentTarget;
if(node.disabled){
return;
}
this.clickedButton=node;
},formFilter:function(node){
var type=(node.type||"").toLowerCase();
var _291=false;
if(node.disabled||!node.name){
_291=false;
}else{
if(dojo.lang.inArray(["submit","button","image"],type)){
if(!this.clickedButton){
this.clickedButton=node;
}
_291=node==this.clickedButton;
}else{
_291=!dojo.lang.inArray(["file","submit","reset","button"],type);
}
}
return _291;
},connect:function(_292,_293,_294){
if(dojo.evalObjPath("dojo.event.connect")){
dojo.event.connect(_292,_293,this,_294);
}else{
var fcn=dojo.lang.hitch(this,_294);
_292[_293]=function(e){
if(!e){
e=window.event;
}
if(!e.currentTarget){
e.currentTarget=e.srcElement;
}
if(!e.preventDefault){
e.preventDefault=function(){
window.event.returnValue=false;
};
}
fcn(e);
};
}
}});
dojo.io.XMLHTTPTransport=new function(){
var _297=this;
var _298={};
this.useCache=false;
this.preventCache=false;
function getCacheKey(url,_29a,_29b){
return url+"|"+_29a+"|"+_29b.toLowerCase();
}
function addToCache(url,_29d,_29e,http){
_298[getCacheKey(url,_29d,_29e)]=http;
}
function getFromCache(url,_2a1,_2a2){
return _298[getCacheKey(url,_2a1,_2a2)];
}
this.clearCache=function(){
_298={};
};
function doLoad(_2a3,http,url,_2a6,_2a7){
if(((http.status>=200)&&(http.status<300))||(http.status==304)||(location.protocol=="file:"&&(http.status==0||http.status==undefined))||(location.protocol=="chrome:"&&(http.status==0||http.status==undefined))){
var ret;
if(_2a3.method.toLowerCase()=="head"){
var _2a9=http.getAllResponseHeaders();
ret={};
ret.toString=function(){
return _2a9;
};
var _2aa=_2a9.split(/[\r\n]+/g);
for(var i=0;i<_2aa.length;i++){
var pair=_2aa[i].match(/^([^:]+)\s*:\s*(.+)$/i);
if(pair){
ret[pair[1]]=pair[2];
}
}
}else{
if(_2a3.mimetype=="text/javascript"){
try{
ret=dj_eval(http.responseText);
}
catch(e){
dojo.debug(e);
dojo.debug(http.responseText);
ret=null;
}
}else{
if(_2a3.mimetype=="text/json"||_2a3.mimetype=="application/json"){
try{
ret=dj_eval("("+http.responseText+")");
}
catch(e){
dojo.debug(e);
dojo.debug(http.responseText);
ret=false;
}
}else{
if((_2a3.mimetype=="application/xml")||(_2a3.mimetype=="text/xml")){
ret=http.responseXML;
if(!ret||typeof ret=="string"||!http.getResponseHeader("Content-Type")){
ret=dojo.dom.createDocumentFromText(http.responseText);
}
}else{
ret=http.responseText;
}
}
}
}
if(_2a7){
addToCache(url,_2a6,_2a3.method,http);
}
_2a3[(typeof _2a3.load=="function")?"load":"handle"]("load",ret,http,_2a3);
}else{
var _2ad=new dojo.io.Error("XMLHttpTransport Error: "+http.status+" "+http.statusText);
_2a3[(typeof _2a3.error=="function")?"error":"handle"]("error",_2ad,http,_2a3);
}
}
function setHeaders(http,_2af){
if(_2af["headers"]){
for(var _2b0 in _2af["headers"]){
if(_2b0.toLowerCase()=="content-type"&&!_2af["contentType"]){
_2af["contentType"]=_2af["headers"][_2b0];
}else{
http.setRequestHeader(_2b0,_2af["headers"][_2b0]);
}
}
}
}
this.inFlight=[];
this.inFlightTimer=null;
this.startWatchingInFlight=function(){
if(!this.inFlightTimer){
this.inFlightTimer=setTimeout("dojo.io.XMLHTTPTransport.watchInFlight();",10);
}
};
this.watchInFlight=function(){
var now=null;
if(!dojo.hostenv._blockAsync&&!_297._blockAsync){
for(var x=this.inFlight.length-1;x>=0;x--){
try{
var tif=this.inFlight[x];
if(!tif||tif.http._aborted||!tif.http.readyState){
this.inFlight.splice(x,1);
continue;
}
if(4==tif.http.readyState){
this.inFlight.splice(x,1);
doLoad(tif.req,tif.http,tif.url,tif.query,tif.useCache);
}else{
if(tif.startTime){
if(!now){
now=(new Date()).getTime();
}
if(tif.startTime+(tif.req.timeoutSeconds*1000)<now){
if(typeof tif.http.abort=="function"){
tif.http.abort();
}
this.inFlight.splice(x,1);
tif.req[(typeof tif.req.timeout=="function")?"timeout":"handle"]("timeout",null,tif.http,tif.req);
}
}
}
}
catch(e){
try{
var _2b4=new dojo.io.Error("XMLHttpTransport.watchInFlight Error: "+e);
tif.req[(typeof tif.req.error=="function")?"error":"handle"]("error",_2b4,tif.http,tif.req);
}
catch(e2){
dojo.debug("XMLHttpTransport error callback failed: "+e2);
}
}
}
}
clearTimeout(this.inFlightTimer);
if(this.inFlight.length==0){
this.inFlightTimer=null;
return;
}
this.inFlightTimer=setTimeout("dojo.io.XMLHTTPTransport.watchInFlight();",10);
};
var _2b5=dojo.hostenv.getXmlhttpObject()?true:false;
this.canHandle=function(_2b6){
return _2b5&&dojo.lang.inArray(["text/plain","text/html","application/xml","text/xml","text/javascript","text/json","application/json"],(_2b6["mimetype"].toLowerCase()||""))&&!(_2b6["formNode"]&&dojo.io.formHasFile(_2b6["formNode"]));
};
this.multipartBoundary="45309FFF-BD65-4d50-99C9-36986896A96F";
this.bind=function(_2b7){
if(!_2b7["url"]){
if(!_2b7["formNode"]&&(_2b7["backButton"]||_2b7["back"]||_2b7["changeUrl"]||_2b7["watchForURL"])&&(!djConfig.preventBackButtonFix)){
dojo.deprecated("Using dojo.io.XMLHTTPTransport.bind() to add to browser history without doing an IO request","Use dojo.undo.browser.addToHistory() instead.","0.4");
dojo.undo.browser.addToHistory(_2b7);
return true;
}
}
var url=_2b7.url;
var _2b9="";
if(_2b7["formNode"]){
var ta=_2b7.formNode.getAttribute("action");
if((ta)&&(!_2b7["url"])){
url=ta;
}
var tp=_2b7.formNode.getAttribute("method");
if((tp)&&(!_2b7["method"])){
_2b7.method=tp;
}
_2b9+=dojo.io.encodeForm(_2b7.formNode,_2b7.encoding,_2b7["formFilter"]);
}
if(url.indexOf("#")>-1){
dojo.debug("Warning: dojo.io.bind: stripping hash values from url:",url);
url=url.split("#")[0];
}
if(_2b7["file"]){
_2b7.method="post";
}
if(!_2b7["method"]){
_2b7.method="get";
}
if(_2b7.method.toLowerCase()=="get"){
_2b7.multipart=false;
}else{
if(_2b7["file"]){
_2b7.multipart=true;
}else{
if(!_2b7["multipart"]){
_2b7.multipart=false;
}
}
}
if(_2b7["backButton"]||_2b7["back"]||_2b7["changeUrl"]){
dojo.undo.browser.addToHistory(_2b7);
}
var _2bc=_2b7["content"]||{};
if(_2b7.sendTransport){
_2bc["dojo.transport"]="xmlhttp";
}
do{
if(_2b7.postContent){
_2b9=_2b7.postContent;
break;
}
if(_2bc){
_2b9+=dojo.io.argsFromMap(_2bc,_2b7.encoding);
}
if(_2b7.method.toLowerCase()=="get"||!_2b7.multipart){
break;
}
var t=[];
if(_2b9.length){
var q=_2b9.split("&");
for(var i=0;i<q.length;++i){
if(q[i].length){
var p=q[i].split("=");
t.push("--"+this.multipartBoundary,"Content-Disposition: form-data; name=\""+p[0]+"\"","",p[1]);
}
}
}
if(_2b7.file){
if(dojo.lang.isArray(_2b7.file)){
for(var i=0;i<_2b7.file.length;++i){
var o=_2b7.file[i];
t.push("--"+this.multipartBoundary,"Content-Disposition: form-data; name=\""+o.name+"\"; filename=\""+("fileName" in o?o.fileName:o.name)+"\"","Content-Type: "+("contentType" in o?o.contentType:"application/octet-stream"),"",o.content);
}
}else{
var o=_2b7.file;
t.push("--"+this.multipartBoundary,"Content-Disposition: form-data; name=\""+o.name+"\"; filename=\""+("fileName" in o?o.fileName:o.name)+"\"","Content-Type: "+("contentType" in o?o.contentType:"application/octet-stream"),"",o.content);
}
}
if(t.length){
t.push("--"+this.multipartBoundary+"--","");
_2b9=t.join("\r\n");
}
}while(false);
var _2c2=_2b7["sync"]?false:true;
var _2c3=_2b7["preventCache"]||(this.preventCache==true&&_2b7["preventCache"]!=false);
var _2c4=_2b7["useCache"]==true||(this.useCache==true&&_2b7["useCache"]!=false);
if(!_2c3&&_2c4){
var _2c5=getFromCache(url,_2b9,_2b7.method);
if(_2c5){
doLoad(_2b7,_2c5,url,_2b9,false);
return;
}
}
var http=dojo.hostenv.getXmlhttpObject(_2b7);
var _2c7=false;
if(_2c2){
var _2c8=this.inFlight.push({"req":_2b7,"http":http,"url":url,"query":_2b9,"useCache":_2c4,"startTime":_2b7.timeoutSeconds?(new Date()).getTime():0});
this.startWatchingInFlight();
}else{
_297._blockAsync=true;
}
if(_2b7.method.toLowerCase()=="post"){
if(!_2b7.user){
http.open("POST",url,_2c2);
}else{
http.open("POST",url,_2c2,_2b7.user,_2b7.password);
}
setHeaders(http,_2b7);
http.setRequestHeader("Content-Type",_2b7.multipart?("multipart/form-data; boundary="+this.multipartBoundary):(_2b7.contentType||"application/x-www-form-urlencoded"));
try{
http.send(_2b9);
}
catch(e){
if(typeof http.abort=="function"){
http.abort();
}
doLoad(_2b7,{status:404},url,_2b9,_2c4);
}
}else{
var _2c9=url;
if(_2b9!=""){
_2c9+=(_2c9.indexOf("?")>-1?"&":"?")+_2b9;
}
if(_2c3){
_2c9+=(dojo.string.endsWithAny(_2c9,"?","&")?"":(_2c9.indexOf("?")>-1?"&":"?"))+"dojo.preventCache="+new Date().valueOf();
}
if(!_2b7.user){
http.open(_2b7.method.toUpperCase(),_2c9,_2c2);
}else{
http.open(_2b7.method.toUpperCase(),_2c9,_2c2,_2b7.user,_2b7.password);
}
setHeaders(http,_2b7);
try{
http.send(null);
}
catch(e){
if(typeof http.abort=="function"){
http.abort();
}
doLoad(_2b7,{status:404},url,_2b9,_2c4);
}
}
if(!_2c2){
doLoad(_2b7,http,url,_2b9,_2c4);
_297._blockAsync=false;
}
_2b7.abort=function(){
try{
http._aborted=true;
}
catch(e){
}
return http.abort();
};
return;
};
dojo.io.transports.addTransport("XMLHTTPTransport");
};
}
dojo.provide("dojo.io.cookie");
dojo.io.cookie.setCookie=function(name,_2cb,days,path,_2ce,_2cf){
var _2d0=-1;
if((typeof days=="number")&&(days>=0)){
var d=new Date();
d.setTime(d.getTime()+(days*24*60*60*1000));
_2d0=d.toGMTString();
}
_2cb=escape(_2cb);
document.cookie=name+"="+_2cb+";"+(_2d0!=-1?" expires="+_2d0+";":"")+(path?"path="+path:"")+(_2ce?"; domain="+_2ce:"")+(_2cf?"; secure":"");
};
dojo.io.cookie.set=dojo.io.cookie.setCookie;
dojo.io.cookie.getCookie=function(name){
var idx=document.cookie.lastIndexOf(name+"=");
if(idx==-1){
return null;
}
var _2d4=document.cookie.substring(idx+name.length+1);
var end=_2d4.indexOf(";");
if(end==-1){
end=_2d4.length;
}
_2d4=_2d4.substring(0,end);
_2d4=unescape(_2d4);
return _2d4;
};
dojo.io.cookie.get=dojo.io.cookie.getCookie;
dojo.io.cookie.deleteCookie=function(name){
dojo.io.cookie.setCookie(name,"-",0);
};
dojo.io.cookie.setObjectCookie=function(name,obj,days,path,_2db,_2dc,_2dd){
if(arguments.length==5){
_2dd=_2db;
_2db=null;
_2dc=null;
}
var _2de=[],_2df,_2e0="";
if(!_2dd){
_2df=dojo.io.cookie.getObjectCookie(name);
}
if(days>=0){
if(!_2df){
_2df={};
}
for(var prop in obj){
if(obj[prop]==null){
delete _2df[prop];
}else{
if((typeof obj[prop]=="string")||(typeof obj[prop]=="number")){
_2df[prop]=obj[prop];
}
}
}
prop=null;
for(var prop in _2df){
_2de.push(escape(prop)+"="+escape(_2df[prop]));
}
_2e0=_2de.join("&");
}
dojo.io.cookie.setCookie(name,_2e0,days,path,_2db,_2dc);
};
dojo.io.cookie.getObjectCookie=function(name){
var _2e3=null,_2e4=dojo.io.cookie.getCookie(name);
if(_2e4){
_2e3={};
var _2e5=_2e4.split("&");
for(var i=0;i<_2e5.length;i++){
var pair=_2e5[i].split("=");
var _2e8=pair[1];
if(isNaN(_2e8)){
_2e8=unescape(pair[1]);
}
_2e3[unescape(pair[0])]=_2e8;
}
}
return _2e3;
};
dojo.io.cookie.isSupported=function(){
if(typeof navigator.cookieEnabled!="boolean"){
dojo.io.cookie.setCookie("__TestingYourBrowserForCookieSupport__","CookiesAllowed",90,null);
var _2e9=dojo.io.cookie.getCookie("__TestingYourBrowserForCookieSupport__");
navigator.cookieEnabled=(_2e9=="CookiesAllowed");
if(navigator.cookieEnabled){
this.deleteCookie("__TestingYourBrowserForCookieSupport__");
}
}
return navigator.cookieEnabled;
};
if(!dojo.io.cookies){
dojo.io.cookies=dojo.io.cookie;
}
dojo.kwCompoundRequire({common:["dojo.io.common"],rhino:["dojo.io.RhinoIO"],browser:["dojo.io.BrowserIO","dojo.io.cookie"],dashboard:["dojo.io.BrowserIO","dojo.io.cookie"]});
dojo.provide("dojo.io.*");
dojo.provide("dojo.event.common");
dojo.event=new function(){
this._canTimeout=dojo.lang.isFunction(dj_global["setTimeout"])||dojo.lang.isAlien(dj_global["setTimeout"]);
function interpolateArgs(args,_2eb){
var dl=dojo.lang;
var ao={srcObj:dj_global,srcFunc:null,adviceObj:dj_global,adviceFunc:null,aroundObj:null,aroundFunc:null,adviceType:(args.length>2)?args[0]:"after",precedence:"last",once:false,delay:null,rate:0,adviceMsg:false,maxCalls:-1};
switch(args.length){
case 0:
return;
case 1:
return;
case 2:
ao.srcFunc=args[0];
ao.adviceFunc=args[1];
break;
case 3:
if((dl.isObject(args[0]))&&(dl.isString(args[1]))&&(dl.isString(args[2]))){
ao.adviceType="after";
ao.srcObj=args[0];
ao.srcFunc=args[1];
ao.adviceFunc=args[2];
}else{
if((dl.isString(args[1]))&&(dl.isString(args[2]))){
ao.srcFunc=args[1];
ao.adviceFunc=args[2];
}else{
if((dl.isObject(args[0]))&&(dl.isString(args[1]))&&(dl.isFunction(args[2]))){
ao.adviceType="after";
ao.srcObj=args[0];
ao.srcFunc=args[1];
var _2ee=dl.nameAnonFunc(args[2],ao.adviceObj,_2eb);
ao.adviceFunc=_2ee;
}else{
if((dl.isFunction(args[0]))&&(dl.isObject(args[1]))&&(dl.isString(args[2]))){
ao.adviceType="after";
ao.srcObj=dj_global;
var _2ee=dl.nameAnonFunc(args[0],ao.srcObj,_2eb);
ao.srcFunc=_2ee;
ao.adviceObj=args[1];
ao.adviceFunc=args[2];
}
}
}
}
break;
case 4:
if((dl.isObject(args[0]))&&(dl.isObject(args[2]))){
ao.adviceType="after";
ao.srcObj=args[0];
ao.srcFunc=args[1];
ao.adviceObj=args[2];
ao.adviceFunc=args[3];
}else{
if((dl.isString(args[0]))&&(dl.isString(args[1]))&&(dl.isObject(args[2]))){
ao.adviceType=args[0];
ao.srcObj=dj_global;
ao.srcFunc=args[1];
ao.adviceObj=args[2];
ao.adviceFunc=args[3];
}else{
if((dl.isString(args[0]))&&(dl.isFunction(args[1]))&&(dl.isObject(args[2]))){
ao.adviceType=args[0];
ao.srcObj=dj_global;
var _2ee=dl.nameAnonFunc(args[1],dj_global,_2eb);
ao.srcFunc=_2ee;
ao.adviceObj=args[2];
ao.adviceFunc=args[3];
}else{
if((dl.isString(args[0]))&&(dl.isObject(args[1]))&&(dl.isString(args[2]))&&(dl.isFunction(args[3]))){
ao.srcObj=args[1];
ao.srcFunc=args[2];
var _2ee=dl.nameAnonFunc(args[3],dj_global,_2eb);
ao.adviceObj=dj_global;
ao.adviceFunc=_2ee;
}else{
if(dl.isObject(args[1])){
ao.srcObj=args[1];
ao.srcFunc=args[2];
ao.adviceObj=dj_global;
ao.adviceFunc=args[3];
}else{
if(dl.isObject(args[2])){
ao.srcObj=dj_global;
ao.srcFunc=args[1];
ao.adviceObj=args[2];
ao.adviceFunc=args[3];
}else{
ao.srcObj=ao.adviceObj=ao.aroundObj=dj_global;
ao.srcFunc=args[1];
ao.adviceFunc=args[2];
ao.aroundFunc=args[3];
}
}
}
}
}
}
break;
case 6:
ao.srcObj=args[1];
ao.srcFunc=args[2];
ao.adviceObj=args[3];
ao.adviceFunc=args[4];
ao.aroundFunc=args[5];
ao.aroundObj=dj_global;
break;
default:
ao.srcObj=args[1];
ao.srcFunc=args[2];
ao.adviceObj=args[3];
ao.adviceFunc=args[4];
ao.aroundObj=args[5];
ao.aroundFunc=args[6];
ao.once=args[7];
ao.delay=args[8];
ao.rate=args[9];
ao.adviceMsg=args[10];
ao.maxCalls=(!isNaN(parseInt(args[11])))?args[11]:-1;
break;
}
if(dl.isFunction(ao.aroundFunc)){
var _2ee=dl.nameAnonFunc(ao.aroundFunc,ao.aroundObj,_2eb);
ao.aroundFunc=_2ee;
}
if(dl.isFunction(ao.srcFunc)){
ao.srcFunc=dl.getNameInObj(ao.srcObj,ao.srcFunc);
}
if(dl.isFunction(ao.adviceFunc)){
ao.adviceFunc=dl.getNameInObj(ao.adviceObj,ao.adviceFunc);
}
if((ao.aroundObj)&&(dl.isFunction(ao.aroundFunc))){
ao.aroundFunc=dl.getNameInObj(ao.aroundObj,ao.aroundFunc);
}
if(!ao.srcObj){
dojo.raise("bad srcObj for srcFunc: "+ao.srcFunc);
}
if(!ao.adviceObj){
dojo.raise("bad adviceObj for adviceFunc: "+ao.adviceFunc);
}
if(!ao.adviceFunc){
dojo.debug("bad adviceFunc for srcFunc: "+ao.srcFunc);
dojo.debugShallow(ao);
}
return ao;
}
this.connect=function(){
if(arguments.length==1){
var ao=arguments[0];
}else{
var ao=interpolateArgs(arguments,true);
}
if(dojo.lang.isArray(ao.srcObj)&&ao.srcObj!=""){
var _2f0={};
for(var x in ao){
_2f0[x]=ao[x];
}
var mjps=[];
dojo.lang.forEach(ao.srcObj,function(src){
if((dojo.render.html.capable)&&(dojo.lang.isString(src))){
src=dojo.byId(src);
}
_2f0.srcObj=src;
mjps.push(dojo.event.connect.call(dojo.event,_2f0));
});
return mjps;
}
var mjp=dojo.event.MethodJoinPoint.getForMethod(ao.srcObj,ao.srcFunc);
if(ao.adviceFunc){
var mjp2=dojo.event.MethodJoinPoint.getForMethod(ao.adviceObj,ao.adviceFunc);
}
mjp.kwAddAdvice(ao);
return mjp;
};
this.log=function(a1,a2){
var _2f8;
if((arguments.length==1)&&(typeof a1=="object")){
_2f8=a1;
}else{
_2f8={srcObj:a1,srcFunc:a2};
}
_2f8.adviceFunc=function(){
var _2f9=[];
for(var x=0;x<arguments.length;x++){
_2f9.push(arguments[x]);
}
dojo.debug("("+_2f8.srcObj+")."+_2f8.srcFunc,":",_2f9.join(", "));
};
this.kwConnect(_2f8);
};
this.connectBefore=function(){
var args=["before"];
for(var i=0;i<arguments.length;i++){
args.push(arguments[i]);
}
return this.connect.apply(this,args);
};
this.connectAround=function(){
var args=["around"];
for(var i=0;i<arguments.length;i++){
args.push(arguments[i]);
}
return this.connect.apply(this,args);
};
this.connectOnce=function(){
var ao=interpolateArgs(arguments,true);
ao.once=true;
return this.connect(ao);
};
this.connectRunOnce=function(){
var ao=interpolateArgs(arguments,true);
ao.maxCalls=1;
return this.connect(ao);
};
this._kwConnectImpl=function(_301,_302){
var fn=(_302)?"disconnect":"connect";
if(typeof _301["srcFunc"]=="function"){
_301.srcObj=_301["srcObj"]||dj_global;
var _304=dojo.lang.nameAnonFunc(_301.srcFunc,_301.srcObj,true);
_301.srcFunc=_304;
}
if(typeof _301["adviceFunc"]=="function"){
_301.adviceObj=_301["adviceObj"]||dj_global;
var _304=dojo.lang.nameAnonFunc(_301.adviceFunc,_301.adviceObj,true);
_301.adviceFunc=_304;
}
_301.srcObj=_301["srcObj"]||dj_global;
_301.adviceObj=_301["adviceObj"]||_301["targetObj"]||dj_global;
_301.adviceFunc=_301["adviceFunc"]||_301["targetFunc"];
return dojo.event[fn](_301);
};
this.kwConnect=function(_305){
return this._kwConnectImpl(_305,false);
};
this.disconnect=function(){
if(arguments.length==1){
var ao=arguments[0];
}else{
var ao=interpolateArgs(arguments,true);
}
if(!ao.adviceFunc){
return;
}
if(dojo.lang.isString(ao.srcFunc)&&(ao.srcFunc.toLowerCase()=="onkey")){
if(dojo.render.html.ie){
ao.srcFunc="onkeydown";
this.disconnect(ao);
}
ao.srcFunc="onkeypress";
}
if(!ao.srcObj[ao.srcFunc]){
return null;
}
var mjp=dojo.event.MethodJoinPoint.getForMethod(ao.srcObj,ao.srcFunc,true);
mjp.removeAdvice(ao.adviceObj,ao.adviceFunc,ao.adviceType,ao.once);
return mjp;
};
this.kwDisconnect=function(_308){
return this._kwConnectImpl(_308,true);
};
};
dojo.event.MethodInvocation=function(_309,obj,args){
this.jp_=_309;
this.object=obj;
this.args=[];
for(var x=0;x<args.length;x++){
this.args[x]=args[x];
}
this.around_index=-1;
};
dojo.event.MethodInvocation.prototype.proceed=function(){
this.around_index++;
if(this.around_index>=this.jp_.around.length){
return this.jp_.object[this.jp_.methodname].apply(this.jp_.object,this.args);
}else{
var ti=this.jp_.around[this.around_index];
var mobj=ti[0]||dj_global;
var meth=ti[1];
return mobj[meth].call(mobj,this);
}
};
dojo.event.MethodJoinPoint=function(obj,_311){
this.object=obj||dj_global;
this.methodname=_311;
this.methodfunc=this.object[_311];
this.squelch=false;
};
dojo.event.MethodJoinPoint.getForMethod=function(obj,_313){
if(!obj){
obj=dj_global;
}
var ofn=obj[_313];
if(!ofn){
ofn=obj[_313]=function(){
};
if(!obj[_313]){
dojo.raise("Cannot set do-nothing method on that object "+_313);
}
}else{
if((typeof ofn!="function")&&(!dojo.lang.isFunction(ofn))&&(!dojo.lang.isAlien(ofn))){
return null;
}
}
var _315=_313+"$joinpoint";
var _316=_313+"$joinpoint$method";
var _317=obj[_315];
if(!_317){
var _318=false;
if(dojo.event["browser"]){
if((obj["attachEvent"])||(obj["nodeType"])||(obj["addEventListener"])){
_318=true;
dojo.event.browser.addClobberNodeAttrs(obj,[_315,_316,_313]);
}
}
var _319=ofn.length;
obj[_316]=ofn;
_317=obj[_315]=new dojo.event.MethodJoinPoint(obj,_316);
if(!_318){
obj[_313]=function(){
return _317.run.apply(_317,arguments);
};
}else{
obj[_313]=function(){
var args=[];
if(!arguments.length){
var evt=null;
try{
if(obj.ownerDocument){
evt=obj.ownerDocument.parentWindow.event;
}else{
if(obj.documentElement){
evt=obj.documentElement.ownerDocument.parentWindow.event;
}else{
if(obj.event){
evt=obj.event;
}else{
evt=window.event;
}
}
}
}
catch(e){
evt=window.event;
}
if(evt){
args.push(dojo.event.browser.fixEvent(evt,this));
}
}else{
for(var x=0;x<arguments.length;x++){
if((x==0)&&(dojo.event.browser.isEvent(arguments[x]))){
args.push(dojo.event.browser.fixEvent(arguments[x],this));
}else{
args.push(arguments[x]);
}
}
}
return _317.run.apply(_317,args);
};
}
obj[_313].__preJoinArity=_319;
}
return _317;
};
dojo.lang.extend(dojo.event.MethodJoinPoint,{squelch:false,unintercept:function(){
this.object[this.methodname]=this.methodfunc;
this.before=[];
this.after=[];
this.around=[];
},disconnect:dojo.lang.forward("unintercept"),run:function(){
var obj=this.object||dj_global;
var args=arguments;
var _31f=[];
for(var x=0;x<args.length;x++){
_31f[x]=args[x];
}
var _321=function(marr){
if(!marr){
dojo.debug("Null argument to unrollAdvice()");
return;
}
var _323=marr[0]||dj_global;
var _324=marr[1];
if(!_323[_324]){
dojo.raise("function \""+_324+"\" does not exist on \""+_323+"\"");
}
var _325=marr[2]||dj_global;
var _326=marr[3];
var msg=marr[6];
var _328=marr[7];
if(_328>-1){
if(_328==0){
return;
}
marr[7]--;
}
var _329;
var to={args:[],jp_:this,object:obj,proceed:function(){
return _323[_324].apply(_323,to.args);
}};
to.args=_31f;
var _32b=parseInt(marr[4]);
var _32c=((!isNaN(_32b))&&(marr[4]!==null)&&(typeof marr[4]!="undefined"));
if(marr[5]){
var rate=parseInt(marr[5]);
var cur=new Date();
var _32f=false;
if((marr["last"])&&((cur-marr.last)<=rate)){
if(dojo.event._canTimeout){
if(marr["delayTimer"]){
clearTimeout(marr.delayTimer);
}
var tod=parseInt(rate*2);
var mcpy=dojo.lang.shallowCopy(marr);
marr.delayTimer=setTimeout(function(){
mcpy[5]=0;
_321(mcpy);
},tod);
}
return;
}else{
marr.last=cur;
}
}
if(_326){
_325[_326].call(_325,to);
}else{
if((_32c)&&((dojo.render.html)||(dojo.render.svg))){
dj_global["setTimeout"](function(){
if(msg){
_323[_324].call(_323,to);
}else{
_323[_324].apply(_323,args);
}
},_32b);
}else{
if(msg){
_323[_324].call(_323,to);
}else{
_323[_324].apply(_323,args);
}
}
}
};
var _332=function(){
if(this.squelch){
try{
return _321.apply(this,arguments);
}
catch(e){
dojo.debug(e);
}
}else{
return _321.apply(this,arguments);
}
};
if((this["before"])&&(this.before.length>0)){
dojo.lang.forEach(this.before.concat(new Array()),_332);
}
var _333;
try{
if((this["around"])&&(this.around.length>0)){
var mi=new dojo.event.MethodInvocation(this,obj,args);
_333=mi.proceed();
}else{
if(this.methodfunc){
_333=this.object[this.methodname].apply(this.object,args);
}
}
}
catch(e){
if(!this.squelch){
dojo.debug(e,"when calling",this.methodname,"on",this.object,"with arguments",args);
dojo.raise(e);
}
}
if((this["after"])&&(this.after.length>0)){
dojo.lang.forEach(this.after.concat(new Array()),_332);
}
return (this.methodfunc)?_333:null;
},getArr:function(kind){
var type="after";
if((typeof kind=="string")&&(kind.indexOf("before")!=-1)){
type="before";
}else{
if(kind=="around"){
type="around";
}
}
if(!this[type]){
this[type]=[];
}
return this[type];
},kwAddAdvice:function(args){
this.addAdvice(args["adviceObj"],args["adviceFunc"],args["aroundObj"],args["aroundFunc"],args["adviceType"],args["precedence"],args["once"],args["delay"],args["rate"],args["adviceMsg"],args["maxCalls"]);
},addAdvice:function(_338,_339,_33a,_33b,_33c,_33d,once,_33f,rate,_341,_342){
var arr=this.getArr(_33c);
if(!arr){
dojo.raise("bad this: "+this);
}
var ao=[_338,_339,_33a,_33b,_33f,rate,_341,_342];
if(once){
if(this.hasAdvice(_338,_339,_33c,arr)>=0){
return;
}
}
if(_33d=="first"){
arr.unshift(ao);
}else{
arr.push(ao);
}
},hasAdvice:function(_345,_346,_347,arr){
if(!arr){
arr=this.getArr(_347);
}
var ind=-1;
for(var x=0;x<arr.length;x++){
var aao=(typeof _346=="object")?(new String(_346)).toString():_346;
var a1o=(typeof arr[x][1]=="object")?(new String(arr[x][1])).toString():arr[x][1];
if((arr[x][0]==_345)&&(a1o==aao)){
ind=x;
}
}
return ind;
},removeAdvice:function(_34d,_34e,_34f,once){
var arr=this.getArr(_34f);
var ind=this.hasAdvice(_34d,_34e,_34f,arr);
if(ind==-1){
return false;
}
while(ind!=-1){
arr.splice(ind,1);
if(once){
break;
}
ind=this.hasAdvice(_34d,_34e,_34f,arr);
}
return true;
}});
dojo.provide("dojo.event.topic");
dojo.event.topic=new function(){
this.topics={};
this.getTopic=function(_353){
if(!this.topics[_353]){
this.topics[_353]=new this.TopicImpl(_353);
}
return this.topics[_353];
};
this.registerPublisher=function(_354,obj,_356){
var _354=this.getTopic(_354);
_354.registerPublisher(obj,_356);
};
this.subscribe=function(_357,obj,_359){
var _357=this.getTopic(_357);
_357.subscribe(obj,_359);
};
this.unsubscribe=function(_35a,obj,_35c){
var _35a=this.getTopic(_35a);
_35a.unsubscribe(obj,_35c);
};
this.destroy=function(_35d){
this.getTopic(_35d).destroy();
delete this.topics[_35d];
};
this.publishApply=function(_35e,args){
var _35e=this.getTopic(_35e);
_35e.sendMessage.apply(_35e,args);
};
this.publish=function(_360,_361){
var _360=this.getTopic(_360);
var args=[];
for(var x=1;x<arguments.length;x++){
args.push(arguments[x]);
}
_360.sendMessage.apply(_360,args);
};
};
dojo.event.topic.TopicImpl=function(_364){
this.topicName=_364;
this.subscribe=function(_365,_366){
var tf=_366||_365;
var to=(!_366)?dj_global:_365;
return dojo.event.kwConnect({srcObj:this,srcFunc:"sendMessage",adviceObj:to,adviceFunc:tf});
};
this.unsubscribe=function(_369,_36a){
var tf=(!_36a)?_369:_36a;
var to=(!_36a)?null:_369;
return dojo.event.kwDisconnect({srcObj:this,srcFunc:"sendMessage",adviceObj:to,adviceFunc:tf});
};
this._getJoinPoint=function(){
return dojo.event.MethodJoinPoint.getForMethod(this,"sendMessage");
};
this.setSquelch=function(_36d){
this._getJoinPoint().squelch=_36d;
};
this.destroy=function(){
this._getJoinPoint().disconnect();
};
this.registerPublisher=function(_36e,_36f){
dojo.event.connect(_36e,_36f,this,"sendMessage");
};
this.sendMessage=function(_370){
};
};
dojo.provide("dojo.event.browser");
dojo._ie_clobber=new function(){
this.clobberNodes=[];
function nukeProp(node,prop){
try{
node[prop]=null;
}
catch(e){
}
try{
delete node[prop];
}
catch(e){
}
try{
node.removeAttribute(prop);
}
catch(e){
}
}
this.clobber=function(_373){
var na;
var tna;
if(_373){
tna=_373.all||_373.getElementsByTagName("*");
na=[_373];
for(var x=0;x<tna.length;x++){
if(tna[x]["__doClobber__"]){
na.push(tna[x]);
}
}
}else{
try{
window.onload=null;
}
catch(e){
}
na=(this.clobberNodes.length)?this.clobberNodes:document.all;
}
tna=null;
var _377={};
for(var i=na.length-1;i>=0;i=i-1){
var el=na[i];
try{
if(el&&el["__clobberAttrs__"]){
for(var j=0;j<el.__clobberAttrs__.length;j++){
nukeProp(el,el.__clobberAttrs__[j]);
}
nukeProp(el,"__clobberAttrs__");
nukeProp(el,"__doClobber__");
}
}
catch(e){
}
}
na=null;
};
};
if(dojo.render.html.ie){
dojo.addOnUnload(function(){
dojo._ie_clobber.clobber();
try{
if((dojo["widget"])&&(dojo.widget["manager"])){
dojo.widget.manager.destroyAll();
}
}
catch(e){
}
if(dojo.widget){
for(var name in dojo.widget._templateCache){
if(dojo.widget._templateCache[name].node){
dojo.dom.destroyNode(dojo.widget._templateCache[name].node);
dojo.widget._templateCache[name].node=null;
delete dojo.widget._templateCache[name].node;
}
}
}
try{
window.onload=null;
}
catch(e){
}
try{
window.onunload=null;
}
catch(e){
}
dojo._ie_clobber.clobberNodes=[];
});
}
dojo.event.browser=new function(){
var _37c=0;
this.normalizedEventName=function(_37d){
switch(_37d){
case "CheckboxStateChange":
case "DOMAttrModified":
case "DOMMenuItemActive":
case "DOMMenuItemInactive":
case "DOMMouseScroll":
case "DOMNodeInserted":
case "DOMNodeRemoved":
case "RadioStateChange":
return _37d;
break;
default:
var lcn=_37d.toLowerCase();
return (lcn.indexOf("on")==0)?lcn.substr(2):lcn;
break;
}
};
this.clean=function(node){
if(dojo.render.html.ie){
dojo._ie_clobber.clobber(node);
}
};
this.addClobberNode=function(node){
if(!dojo.render.html.ie){
return;
}
if(!node["__doClobber__"]){
node.__doClobber__=true;
dojo._ie_clobber.clobberNodes.push(node);
node.__clobberAttrs__=[];
}
};
this.addClobberNodeAttrs=function(node,_382){
if(!dojo.render.html.ie){
return;
}
this.addClobberNode(node);
for(var x=0;x<_382.length;x++){
node.__clobberAttrs__.push(_382[x]);
}
};
this.removeListener=function(node,_385,fp,_387){
if(!_387){
var _387=false;
}
_385=dojo.event.browser.normalizedEventName(_385);
if(_385=="key"){
if(dojo.render.html.ie){
this.removeListener(node,"onkeydown",fp,_387);
}
_385="keypress";
}
if(node.removeEventListener){
node.removeEventListener(_385,fp,_387);
}
};
this.addListener=function(node,_389,fp,_38b,_38c){
if(!node){
return;
}
if(!_38b){
var _38b=false;
}
_389=dojo.event.browser.normalizedEventName(_389);
if(_389=="key"){
if(dojo.render.html.ie){
this.addListener(node,"onkeydown",fp,_38b,_38c);
}
_389="keypress";
}
if(!_38c){
var _38d=function(evt){
if(!evt){
evt=window.event;
}
var ret=fp(dojo.event.browser.fixEvent(evt,this));
if(_38b){
dojo.event.browser.stopEvent(evt);
}
return ret;
};
}else{
_38d=fp;
}
if(node.addEventListener){
node.addEventListener(_389,_38d,_38b);
return _38d;
}else{
_389="on"+_389;
if(typeof node[_389]=="function"){
var _390=node[_389];
node[_389]=function(e){
_390(e);
return _38d(e);
};
}else{
node[_389]=_38d;
}
if(dojo.render.html.ie){
this.addClobberNodeAttrs(node,[_389]);
}
return _38d;
}
};
this.isEvent=function(obj){
return (typeof obj!="undefined")&&(obj)&&(typeof Event!="undefined")&&(obj.eventPhase);
};
this.currentEvent=null;
this.callListener=function(_393,_394){
if(typeof _393!="function"){
dojo.raise("listener not a function: "+_393);
}
dojo.event.browser.currentEvent.currentTarget=_394;
return _393.call(_394,dojo.event.browser.currentEvent);
};
this._stopPropagation=function(){
dojo.event.browser.currentEvent.cancelBubble=true;
};
this._preventDefault=function(){
dojo.event.browser.currentEvent.returnValue=false;
};
this.keys={KEY_BACKSPACE:8,KEY_TAB:9,KEY_CLEAR:12,KEY_ENTER:13,KEY_SHIFT:16,KEY_CTRL:17,KEY_ALT:18,KEY_PAUSE:19,KEY_CAPS_LOCK:20,KEY_ESCAPE:27,KEY_SPACE:32,KEY_PAGE_UP:33,KEY_PAGE_DOWN:34,KEY_END:35,KEY_HOME:36,KEY_LEFT_ARROW:37,KEY_UP_ARROW:38,KEY_RIGHT_ARROW:39,KEY_DOWN_ARROW:40,KEY_INSERT:45,KEY_DELETE:46,KEY_HELP:47,KEY_LEFT_WINDOW:91,KEY_RIGHT_WINDOW:92,KEY_SELECT:93,KEY_NUMPAD_0:96,KEY_NUMPAD_1:97,KEY_NUMPAD_2:98,KEY_NUMPAD_3:99,KEY_NUMPAD_4:100,KEY_NUMPAD_5:101,KEY_NUMPAD_6:102,KEY_NUMPAD_7:103,KEY_NUMPAD_8:104,KEY_NUMPAD_9:105,KEY_NUMPAD_MULTIPLY:106,KEY_NUMPAD_PLUS:107,KEY_NUMPAD_ENTER:108,KEY_NUMPAD_MINUS:109,KEY_NUMPAD_PERIOD:110,KEY_NUMPAD_DIVIDE:111,KEY_F1:112,KEY_F2:113,KEY_F3:114,KEY_F4:115,KEY_F5:116,KEY_F6:117,KEY_F7:118,KEY_F8:119,KEY_F9:120,KEY_F10:121,KEY_F11:122,KEY_F12:123,KEY_F13:124,KEY_F14:125,KEY_F15:126,KEY_NUM_LOCK:144,KEY_SCROLL_LOCK:145};
this.revKeys=[];
for(var key in this.keys){
this.revKeys[this.keys[key]]=key;
}
this.fixEvent=function(evt,_397){
if(!evt){
if(window["event"]){
evt=window.event;
}
}
if((evt["type"])&&(evt["type"].indexOf("key")==0)){
evt.keys=this.revKeys;
for(var key in this.keys){
evt[key]=this.keys[key];
}
if(evt["type"]=="keydown"&&dojo.render.html.ie){
switch(evt.keyCode){
case evt.KEY_SHIFT:
case evt.KEY_CTRL:
case evt.KEY_ALT:
case evt.KEY_CAPS_LOCK:
case evt.KEY_LEFT_WINDOW:
case evt.KEY_RIGHT_WINDOW:
case evt.KEY_SELECT:
case evt.KEY_NUM_LOCK:
case evt.KEY_SCROLL_LOCK:
case evt.KEY_NUMPAD_0:
case evt.KEY_NUMPAD_1:
case evt.KEY_NUMPAD_2:
case evt.KEY_NUMPAD_3:
case evt.KEY_NUMPAD_4:
case evt.KEY_NUMPAD_5:
case evt.KEY_NUMPAD_6:
case evt.KEY_NUMPAD_7:
case evt.KEY_NUMPAD_8:
case evt.KEY_NUMPAD_9:
case evt.KEY_NUMPAD_PERIOD:
break;
case evt.KEY_NUMPAD_MULTIPLY:
case evt.KEY_NUMPAD_PLUS:
case evt.KEY_NUMPAD_ENTER:
case evt.KEY_NUMPAD_MINUS:
case evt.KEY_NUMPAD_DIVIDE:
break;
case evt.KEY_PAUSE:
case evt.KEY_TAB:
case evt.KEY_BACKSPACE:
case evt.KEY_ENTER:
case evt.KEY_ESCAPE:
case evt.KEY_PAGE_UP:
case evt.KEY_PAGE_DOWN:
case evt.KEY_END:
case evt.KEY_HOME:
case evt.KEY_LEFT_ARROW:
case evt.KEY_UP_ARROW:
case evt.KEY_RIGHT_ARROW:
case evt.KEY_DOWN_ARROW:
case evt.KEY_INSERT:
case evt.KEY_DELETE:
case evt.KEY_F1:
case evt.KEY_F2:
case evt.KEY_F3:
case evt.KEY_F4:
case evt.KEY_F5:
case evt.KEY_F6:
case evt.KEY_F7:
case evt.KEY_F8:
case evt.KEY_F9:
case evt.KEY_F10:
case evt.KEY_F11:
case evt.KEY_F12:
case evt.KEY_F12:
case evt.KEY_F13:
case evt.KEY_F14:
case evt.KEY_F15:
case evt.KEY_CLEAR:
case evt.KEY_HELP:
evt.key=evt.keyCode;
break;
default:
if(evt.ctrlKey||evt.altKey){
var _399=evt.keyCode;
if(_399>=65&&_399<=90&&evt.shiftKey==false){
_399+=32;
}
if(_399>=1&&_399<=26&&evt.ctrlKey){
_399+=96;
}
evt.key=String.fromCharCode(_399);
}
}
}else{
if(evt["type"]=="keypress"){
if(dojo.render.html.opera){
if(evt.which==0){
evt.key=evt.keyCode;
}else{
if(evt.which>0){
switch(evt.which){
case evt.KEY_SHIFT:
case evt.KEY_CTRL:
case evt.KEY_ALT:
case evt.KEY_CAPS_LOCK:
case evt.KEY_NUM_LOCK:
case evt.KEY_SCROLL_LOCK:
break;
case evt.KEY_PAUSE:
case evt.KEY_TAB:
case evt.KEY_BACKSPACE:
case evt.KEY_ENTER:
case evt.KEY_ESCAPE:
evt.key=evt.which;
break;
default:
var _399=evt.which;
if((evt.ctrlKey||evt.altKey||evt.metaKey)&&(evt.which>=65&&evt.which<=90&&evt.shiftKey==false)){
_399+=32;
}
evt.key=String.fromCharCode(_399);
}
}
}
}else{
if(dojo.render.html.ie){
if(!evt.ctrlKey&&!evt.altKey&&evt.keyCode>=evt.KEY_SPACE){
evt.key=String.fromCharCode(evt.keyCode);
}
}else{
if(dojo.render.html.safari){
switch(evt.keyCode){
case 25:
evt.key=evt.KEY_TAB;
evt.shift=true;
break;
case 63232:
evt.key=evt.KEY_UP_ARROW;
break;
case 63233:
evt.key=evt.KEY_DOWN_ARROW;
break;
case 63234:
evt.key=evt.KEY_LEFT_ARROW;
break;
case 63235:
evt.key=evt.KEY_RIGHT_ARROW;
break;
case 63236:
evt.key=evt.KEY_F1;
break;
case 63237:
evt.key=evt.KEY_F2;
break;
case 63238:
evt.key=evt.KEY_F3;
break;
case 63239:
evt.key=evt.KEY_F4;
break;
case 63240:
evt.key=evt.KEY_F5;
break;
case 63241:
evt.key=evt.KEY_F6;
break;
case 63242:
evt.key=evt.KEY_F7;
break;
case 63243:
evt.key=evt.KEY_F8;
break;
case 63244:
evt.key=evt.KEY_F9;
break;
case 63245:
evt.key=evt.KEY_F10;
break;
case 63246:
evt.key=evt.KEY_F11;
break;
case 63247:
evt.key=evt.KEY_F12;
break;
case 63250:
evt.key=evt.KEY_PAUSE;
break;
case 63272:
evt.key=evt.KEY_DELETE;
break;
case 63273:
evt.key=evt.KEY_HOME;
break;
case 63275:
evt.key=evt.KEY_END;
break;
case 63276:
evt.key=evt.KEY_PAGE_UP;
break;
case 63277:
evt.key=evt.KEY_PAGE_DOWN;
break;
case 63302:
evt.key=evt.KEY_INSERT;
break;
case 63248:
case 63249:
case 63289:
break;
default:
evt.key=evt.charCode>=evt.KEY_SPACE?String.fromCharCode(evt.charCode):evt.keyCode;
}
}else{
evt.key=evt.charCode>0?String.fromCharCode(evt.charCode):evt.keyCode;
}
}
}
}
}
}
if(dojo.render.html.ie){
if(!evt.target){
evt.target=evt.srcElement;
}
if(!evt.currentTarget){
evt.currentTarget=(_397?_397:evt.srcElement);
}
if(!evt.layerX){
evt.layerX=evt.offsetX;
}
if(!evt.layerY){
evt.layerY=evt.offsetY;
}
var doc=(evt.srcElement&&evt.srcElement.ownerDocument)?evt.srcElement.ownerDocument:document;
var _39b=((dojo.render.html.ie55)||(doc["compatMode"]=="BackCompat"))?doc.body:doc.documentElement;
if(!evt.pageX){
evt.pageX=evt.clientX+(_39b.scrollLeft||0);
}
if(!evt.pageY){
evt.pageY=evt.clientY+(_39b.scrollTop||0);
}
if(evt.type=="mouseover"){
evt.relatedTarget=evt.fromElement;
}
if(evt.type=="mouseout"){
evt.relatedTarget=evt.toElement;
}
this.currentEvent=evt;
evt.callListener=this.callListener;
evt.stopPropagation=this._stopPropagation;
evt.preventDefault=this._preventDefault;
}
return evt;
};
this.stopEvent=function(evt){
if(window.event){
evt.cancelBubble=true;
evt.returnValue=false;
}else{
evt.preventDefault();
evt.stopPropagation();
}
};
};
dojo.kwCompoundRequire({common:["dojo.event.common","dojo.event.topic"],browser:["dojo.event.browser"],dashboard:["dojo.event.browser"]});
dojo.provide("dojo.event.*");
dojo.provide("dojo.gfx.color");
dojo.gfx.color.Color=function(r,g,b,a){
if(dojo.lang.isArray(r)){
this.r=r[0];
this.g=r[1];
this.b=r[2];
this.a=r[3]||1;
}else{
if(dojo.lang.isString(r)){
var rgb=dojo.gfx.color.extractRGB(r);
this.r=rgb[0];
this.g=rgb[1];
this.b=rgb[2];
this.a=g||1;
}else{
if(r instanceof dojo.gfx.color.Color){
this.r=r.r;
this.b=r.b;
this.g=r.g;
this.a=r.a;
}else{
this.r=r;
this.g=g;
this.b=b;
this.a=a;
}
}
}
};
dojo.gfx.color.Color.fromArray=function(arr){
return new dojo.gfx.color.Color(arr[0],arr[1],arr[2],arr[3]);
};
dojo.extend(dojo.gfx.color.Color,{toRgb:function(_3a3){
if(_3a3){
return this.toRgba();
}else{
return [this.r,this.g,this.b];
}
},toRgba:function(){
return [this.r,this.g,this.b,this.a];
},toHex:function(){
return dojo.gfx.color.rgb2hex(this.toRgb());
},toCss:function(){
return "rgb("+this.toRgb().join()+")";
},toString:function(){
return this.toHex();
},blend:function(_3a4,_3a5){
var rgb=null;
if(dojo.lang.isArray(_3a4)){
rgb=_3a4;
}else{
if(_3a4 instanceof dojo.gfx.color.Color){
rgb=_3a4.toRgb();
}else{
rgb=new dojo.gfx.color.Color(_3a4).toRgb();
}
}
return dojo.gfx.color.blend(this.toRgb(),rgb,_3a5);
}});
dojo.gfx.color.named={white:[255,255,255],black:[0,0,0],red:[255,0,0],green:[0,255,0],lime:[0,255,0],blue:[0,0,255],navy:[0,0,128],gray:[128,128,128],silver:[192,192,192]};
dojo.gfx.color.blend=function(a,b,_3a9){
if(typeof a=="string"){
return dojo.gfx.color.blendHex(a,b,_3a9);
}
if(!_3a9){
_3a9=0;
}
_3a9=Math.min(Math.max(-1,_3a9),1);
_3a9=((_3a9+1)/2);
var c=[];
for(var x=0;x<3;x++){
c[x]=parseInt(b[x]+((a[x]-b[x])*_3a9));
}
return c;
};
dojo.gfx.color.blendHex=function(a,b,_3ae){
return dojo.gfx.color.rgb2hex(dojo.gfx.color.blend(dojo.gfx.color.hex2rgb(a),dojo.gfx.color.hex2rgb(b),_3ae));
};
dojo.gfx.color.extractRGB=function(_3af){
var hex="0123456789abcdef";
_3af=_3af.toLowerCase();
if(_3af.indexOf("rgb")==0){
var _3b1=_3af.match(/rgba*\((\d+), *(\d+), *(\d+)/i);
var ret=_3b1.splice(1,3);
return ret;
}else{
var _3b3=dojo.gfx.color.hex2rgb(_3af);
if(_3b3){
return _3b3;
}else{
return dojo.gfx.color.named[_3af]||[255,255,255];
}
}
};
dojo.gfx.color.hex2rgb=function(hex){
var _3b5="0123456789ABCDEF";
var rgb=new Array(3);
if(hex.indexOf("#")==0){
hex=hex.substring(1);
}
hex=hex.toUpperCase();
if(hex.replace(new RegExp("["+_3b5+"]","g"),"")!=""){
return null;
}
if(hex.length==3){
rgb[0]=hex.charAt(0)+hex.charAt(0);
rgb[1]=hex.charAt(1)+hex.charAt(1);
rgb[2]=hex.charAt(2)+hex.charAt(2);
}else{
rgb[0]=hex.substring(0,2);
rgb[1]=hex.substring(2,4);
rgb[2]=hex.substring(4);
}
for(var i=0;i<rgb.length;i++){
rgb[i]=_3b5.indexOf(rgb[i].charAt(0))*16+_3b5.indexOf(rgb[i].charAt(1));
}
return rgb;
};
dojo.gfx.color.rgb2hex=function(r,g,b){
if(dojo.lang.isArray(r)){
g=r[1]||0;
b=r[2]||0;
r=r[0]||0;
}
var ret=dojo.lang.map([r,g,b],function(x){
x=new Number(x);
var s=x.toString(16);
while(s.length<2){
s="0"+s;
}
return s;
});
ret.unshift("#");
return ret.join("");
};
dojo.provide("dojo.lfx.Animation");
dojo.lfx.Line=function(_3be,end){
this.start=_3be;
this.end=end;
if(dojo.lang.isArray(_3be)){
var diff=[];
dojo.lang.forEach(this.start,function(s,i){
diff[i]=this.end[i]-s;
},this);
this.getValue=function(n){
var res=[];
dojo.lang.forEach(this.start,function(s,i){
res[i]=(diff[i]*n)+s;
},this);
return res;
};
}else{
var diff=end-_3be;
this.getValue=function(n){
return (diff*n)+this.start;
};
}
};
if((dojo.render.html.khtml)&&(!dojo.render.html.safari)){
dojo.lfx.easeDefault=function(n){
return (parseFloat("0.5")+((Math.sin((n+parseFloat("1.5"))*Math.PI))/2));
};
}else{
dojo.lfx.easeDefault=function(n){
return (0.5+((Math.sin((n+1.5)*Math.PI))/2));
};
}
dojo.lfx.easeIn=function(n){
return Math.pow(n,3);
};
dojo.lfx.easeOut=function(n){
return (1-Math.pow(1-n,3));
};
dojo.lfx.easeInOut=function(n){
return ((3*Math.pow(n,2))-(2*Math.pow(n,3)));
};
dojo.lfx.IAnimation=function(){
};
dojo.lang.extend(dojo.lfx.IAnimation,{curve:null,duration:1000,easing:null,repeatCount:0,rate:10,handler:null,beforeBegin:null,onBegin:null,onAnimate:null,onEnd:null,onPlay:null,onPause:null,onStop:null,play:null,pause:null,stop:null,connect:function(evt,_3ce,_3cf){
if(!_3cf){
_3cf=_3ce;
_3ce=this;
}
_3cf=dojo.lang.hitch(_3ce,_3cf);
var _3d0=this[evt]||function(){
};
this[evt]=function(){
var ret=_3d0.apply(this,arguments);
_3cf.apply(this,arguments);
return ret;
};
return this;
},fire:function(evt,args){
if(this[evt]){
this[evt].apply(this,(args||[]));
}
return this;
},repeat:function(_3d4){
this.repeatCount=_3d4;
return this;
},_active:false,_paused:false});
dojo.lfx.Animation=function(_3d5,_3d6,_3d7,_3d8,_3d9,rate){
dojo.lfx.IAnimation.call(this);
if(dojo.lang.isNumber(_3d5)||(!_3d5&&_3d6.getValue)){
rate=_3d9;
_3d9=_3d8;
_3d8=_3d7;
_3d7=_3d6;
_3d6=_3d5;
_3d5=null;
}else{
if(_3d5.getValue||dojo.lang.isArray(_3d5)){
rate=_3d8;
_3d9=_3d7;
_3d8=_3d6;
_3d7=_3d5;
_3d6=null;
_3d5=null;
}
}
if(dojo.lang.isArray(_3d7)){
this.curve=new dojo.lfx.Line(_3d7[0],_3d7[1]);
}else{
this.curve=_3d7;
}
if(_3d6!=null&&_3d6>0){
this.duration=_3d6;
}
if(_3d9){
this.repeatCount=_3d9;
}
if(rate){
this.rate=rate;
}
if(_3d5){
dojo.lang.forEach(["handler","beforeBegin","onBegin","onEnd","onPlay","onStop","onAnimate"],function(item){
if(_3d5[item]){
this.connect(item,_3d5[item]);
}
},this);
}
if(_3d8&&dojo.lang.isFunction(_3d8)){
this.easing=_3d8;
}
};
dojo.inherits(dojo.lfx.Animation,dojo.lfx.IAnimation);
dojo.lang.extend(dojo.lfx.Animation,{_startTime:null,_endTime:null,_timer:null,_percent:0,_startRepeatCount:0,play:function(_3dc,_3dd){
if(_3dd){
clearTimeout(this._timer);
this._active=false;
this._paused=false;
this._percent=0;
}else{
if(this._active&&!this._paused){
return this;
}
}
this.fire("handler",["beforeBegin"]);
this.fire("beforeBegin");
if(_3dc>0){
setTimeout(dojo.lang.hitch(this,function(){
this.play(null,_3dd);
}),_3dc);
return this;
}
this._startTime=new Date().valueOf();
if(this._paused){
this._startTime-=(this.duration*this._percent/100);
}
this._endTime=this._startTime+this.duration;
this._active=true;
this._paused=false;
var step=this._percent/100;
var _3df=this.curve.getValue(step);
if(this._percent==0){
if(!this._startRepeatCount){
this._startRepeatCount=this.repeatCount;
}
this.fire("handler",["begin",_3df]);
this.fire("onBegin",[_3df]);
}
this.fire("handler",["play",_3df]);
this.fire("onPlay",[_3df]);
this._cycle();
return this;
},pause:function(){
clearTimeout(this._timer);
if(!this._active){
return this;
}
this._paused=true;
var _3e0=this.curve.getValue(this._percent/100);
this.fire("handler",["pause",_3e0]);
this.fire("onPause",[_3e0]);
return this;
},gotoPercent:function(pct,_3e2){
clearTimeout(this._timer);
this._active=true;
this._paused=true;
this._percent=pct;
if(_3e2){
this.play();
}
return this;
},stop:function(_3e3){
clearTimeout(this._timer);
var step=this._percent/100;
if(_3e3){
step=1;
}
var _3e5=this.curve.getValue(step);
this.fire("handler",["stop",_3e5]);
this.fire("onStop",[_3e5]);
this._active=false;
this._paused=false;
return this;
},status:function(){
if(this._active){
return this._paused?"paused":"playing";
}else{
return "stopped";
}
return this;
},_cycle:function(){
clearTimeout(this._timer);
if(this._active){
var curr=new Date().valueOf();
var step=(curr-this._startTime)/(this._endTime-this._startTime);
if(step>=1){
step=1;
this._percent=100;
}else{
this._percent=step*100;
}
if((this.easing)&&(dojo.lang.isFunction(this.easing))){
step=this.easing(step);
}
var _3e8=this.curve.getValue(step);
this.fire("handler",["animate",_3e8]);
this.fire("onAnimate",[_3e8]);
if(step<1){
this._timer=setTimeout(dojo.lang.hitch(this,"_cycle"),this.rate);
}else{
this._active=false;
this.fire("handler",["end"]);
this.fire("onEnd");
if(this.repeatCount>0){
this.repeatCount--;
this.play(null,true);
}else{
if(this.repeatCount==-1){
this.play(null,true);
}else{
if(this._startRepeatCount){
this.repeatCount=this._startRepeatCount;
this._startRepeatCount=0;
}
}
}
}
}
return this;
}});
dojo.lfx.Combine=function(_3e9){
dojo.lfx.IAnimation.call(this);
this._anims=[];
this._animsEnded=0;
var _3ea=arguments;
if(_3ea.length==1&&(dojo.lang.isArray(_3ea[0])||dojo.lang.isArrayLike(_3ea[0]))){
_3ea=_3ea[0];
}
dojo.lang.forEach(_3ea,function(anim){
this._anims.push(anim);
anim.connect("onEnd",dojo.lang.hitch(this,"_onAnimsEnded"));
},this);
};
dojo.inherits(dojo.lfx.Combine,dojo.lfx.IAnimation);
dojo.lang.extend(dojo.lfx.Combine,{_animsEnded:0,play:function(_3ec,_3ed){
if(!this._anims.length){
return this;
}
this.fire("beforeBegin");
if(_3ec>0){
setTimeout(dojo.lang.hitch(this,function(){
this.play(null,_3ed);
}),_3ec);
return this;
}
if(_3ed||this._anims[0].percent==0){
this.fire("onBegin");
}
this.fire("onPlay");
this._animsCall("play",null,_3ed);
return this;
},pause:function(){
this.fire("onPause");
this._animsCall("pause");
return this;
},stop:function(_3ee){
this.fire("onStop");
this._animsCall("stop",_3ee);
return this;
},_onAnimsEnded:function(){
this._animsEnded++;
if(this._animsEnded>=this._anims.length){
this.fire("onEnd");
}
return this;
},_animsCall:function(_3ef){
var args=[];
if(arguments.length>1){
for(var i=1;i<arguments.length;i++){
args.push(arguments[i]);
}
}
var _3f2=this;
dojo.lang.forEach(this._anims,function(anim){
anim[_3ef](args);
},_3f2);
return this;
}});
dojo.lfx.Chain=function(_3f4){
dojo.lfx.IAnimation.call(this);
this._anims=[];
this._currAnim=-1;
var _3f5=arguments;
if(_3f5.length==1&&(dojo.lang.isArray(_3f5[0])||dojo.lang.isArrayLike(_3f5[0]))){
_3f5=_3f5[0];
}
var _3f6=this;
dojo.lang.forEach(_3f5,function(anim,i,_3f9){
this._anims.push(anim);
if(i<_3f9.length-1){
anim.connect("onEnd",dojo.lang.hitch(this,"_playNext"));
}else{
anim.connect("onEnd",dojo.lang.hitch(this,function(){
this.fire("onEnd");
}));
}
},this);
};
dojo.inherits(dojo.lfx.Chain,dojo.lfx.IAnimation);
dojo.lang.extend(dojo.lfx.Chain,{_currAnim:-1,play:function(_3fa,_3fb){
if(!this._anims.length){
return this;
}
if(_3fb||!this._anims[this._currAnim]){
this._currAnim=0;
}
var _3fc=this._anims[this._currAnim];
this.fire("beforeBegin");
if(_3fa>0){
setTimeout(dojo.lang.hitch(this,function(){
this.play(null,_3fb);
}),_3fa);
return this;
}
if(_3fc){
if(this._currAnim==0){
this.fire("handler",["begin",this._currAnim]);
this.fire("onBegin",[this._currAnim]);
}
this.fire("onPlay",[this._currAnim]);
_3fc.play(null,_3fb);
}
return this;
},pause:function(){
if(this._anims[this._currAnim]){
this._anims[this._currAnim].pause();
this.fire("onPause",[this._currAnim]);
}
return this;
},playPause:function(){
if(this._anims.length==0){
return this;
}
if(this._currAnim==-1){
this._currAnim=0;
}
var _3fd=this._anims[this._currAnim];
if(_3fd){
if(!_3fd._active||_3fd._paused){
this.play();
}else{
this.pause();
}
}
return this;
},stop:function(){
var _3fe=this._anims[this._currAnim];
if(_3fe){
_3fe.stop();
this.fire("onStop",[this._currAnim]);
}
return _3fe;
},_playNext:function(){
if(this._currAnim==-1||this._anims.length==0){
return this;
}
this._currAnim++;
if(this._anims[this._currAnim]){
this._anims[this._currAnim].play(null,true);
}
return this;
}});
dojo.lfx.combine=function(_3ff){
var _400=arguments;
if(dojo.lang.isArray(arguments[0])){
_400=arguments[0];
}
if(_400.length==1){
return _400[0];
}
return new dojo.lfx.Combine(_400);
};
dojo.lfx.chain=function(_401){
var _402=arguments;
if(dojo.lang.isArray(arguments[0])){
_402=arguments[0];
}
if(_402.length==1){
return _402[0];
}
return new dojo.lfx.Chain(_402);
};
dojo.provide("dojo.html.common");
dojo.lang.mixin(dojo.html,dojo.dom);
dojo.html.body=function(){
dojo.deprecated("dojo.html.body() moved to dojo.body()","0.5");
return dojo.body();
};
dojo.html.getEventTarget=function(evt){
if(!evt){
evt=dojo.global().event||{};
}
var t=(evt.srcElement?evt.srcElement:(evt.target?evt.target:null));
while((t)&&(t.nodeType!=1)){
t=t.parentNode;
}
return t;
};
dojo.html.getViewport=function(){
var _405=dojo.global();
var _406=dojo.doc();
var w=0;
var h=0;
if(dojo.render.html.mozilla){
w=_406.documentElement.clientWidth;
h=_405.innerHeight;
}else{
if(!dojo.render.html.opera&&_405.innerWidth){
w=_405.innerWidth;
h=_405.innerHeight;
}else{
if(!dojo.render.html.opera&&dojo.exists(_406,"documentElement.clientWidth")){
var w2=_406.documentElement.clientWidth;
if(!w||w2&&w2<w){
w=w2;
}
h=_406.documentElement.clientHeight;
}else{
if(dojo.body().clientWidth){
w=dojo.body().clientWidth;
h=dojo.body().clientHeight;
}
}
}
}
return {width:w,height:h};
};
dojo.html.getScroll=function(){
var _40a=dojo.global();
var _40b=dojo.doc();
var top=_40a.pageYOffset||_40b.documentElement.scrollTop||dojo.body().scrollTop||0;
var left=_40a.pageXOffset||_40b.documentElement.scrollLeft||dojo.body().scrollLeft||0;
return {top:top,left:left,offset:{x:left,y:top}};
};
dojo.html.getParentByType=function(node,type){
var _410=dojo.doc();
var _411=dojo.byId(node);
type=type.toLowerCase();
while((_411)&&(_411.nodeName.toLowerCase()!=type)){
if(_411==(_410["body"]||_410["documentElement"])){
return null;
}
_411=_411.parentNode;
}
return _411;
};
dojo.html.getAttribute=function(node,attr){
node=dojo.byId(node);
if((!node)||(!node.getAttribute)){
return null;
}
var ta=typeof attr=="string"?attr:new String(attr);
var v=node.getAttribute(ta.toUpperCase());
if((v)&&(typeof v=="string")&&(v!="")){
return v;
}
if(v&&v.value){
return v.value;
}
if((node.getAttributeNode)&&(node.getAttributeNode(ta))){
return (node.getAttributeNode(ta)).value;
}else{
if(node.getAttribute(ta)){
return node.getAttribute(ta);
}else{
if(node.getAttribute(ta.toLowerCase())){
return node.getAttribute(ta.toLowerCase());
}
}
}
return null;
};
dojo.html.hasAttribute=function(node,attr){
return dojo.html.getAttribute(dojo.byId(node),attr)?true:false;
};
dojo.html.getCursorPosition=function(e){
e=e||dojo.global().event;
var _419={x:0,y:0};
if(e.pageX||e.pageY){
_419.x=e.pageX;
_419.y=e.pageY;
}else{
var de=dojo.doc().documentElement;
var db=dojo.body();
_419.x=e.clientX+((de||db)["scrollLeft"])-((de||db)["clientLeft"]);
_419.y=e.clientY+((de||db)["scrollTop"])-((de||db)["clientTop"]);
}
return _419;
};
dojo.html.isTag=function(node){
node=dojo.byId(node);
if(node&&node.tagName){
for(var i=1;i<arguments.length;i++){
if(node.tagName.toLowerCase()==String(arguments[i]).toLowerCase()){
return String(arguments[i]).toLowerCase();
}
}
}
return "";
};
if(dojo.render.html.ie&&!dojo.render.html.ie70){
if(window.location.href.substr(0,6).toLowerCase()!="https:"){
(function(){
var _41e=dojo.doc().createElement("script");
_41e.src="javascript:'dojo.html.createExternalElement=function(doc, tag){ return doc.createElement(tag); }'";
dojo.doc().getElementsByTagName("head")[0].appendChild(_41e);
})();
}
}else{
dojo.html.createExternalElement=function(doc,tag){
return doc.createElement(tag);
};
}
dojo.html._callDeprecated=function(_421,_422,args,_424,_425){
dojo.deprecated("dojo.html."+_421,"replaced by dojo.html."+_422+"("+(_424?"node, {"+_424+": "+_424+"}":"")+")"+(_425?"."+_425:""),"0.5");
var _426=[];
if(_424){
var _427={};
_427[_424]=args[1];
_426.push(args[0]);
_426.push(_427);
}else{
_426=args;
}
var ret=dojo.html[_422].apply(dojo.html,args);
if(_425){
return ret[_425];
}else{
return ret;
}
};
dojo.html.getViewportWidth=function(){
return dojo.html._callDeprecated("getViewportWidth","getViewport",arguments,null,"width");
};
dojo.html.getViewportHeight=function(){
return dojo.html._callDeprecated("getViewportHeight","getViewport",arguments,null,"height");
};
dojo.html.getViewportSize=function(){
return dojo.html._callDeprecated("getViewportSize","getViewport",arguments);
};
dojo.html.getScrollTop=function(){
return dojo.html._callDeprecated("getScrollTop","getScroll",arguments,null,"top");
};
dojo.html.getScrollLeft=function(){
return dojo.html._callDeprecated("getScrollLeft","getScroll",arguments,null,"left");
};
dojo.html.getScrollOffset=function(){
return dojo.html._callDeprecated("getScrollOffset","getScroll",arguments,null,"offset");
};
dojo.provide("dojo.uri.Uri");
dojo.uri=new function(){
this.dojoUri=function(uri){
return new dojo.uri.Uri(dojo.hostenv.getBaseScriptUri(),uri);
};
this.moduleUri=function(_42a,uri){
var loc=dojo.hostenv.getModuleSymbols(_42a).join("/");
if(!loc){
return null;
}
if(loc.lastIndexOf("/")!=loc.length-1){
loc+="/";
}
var _42d=loc.indexOf(":");
var _42e=loc.indexOf("/");
if(loc.charAt(0)!="/"&&(_42d==-1||_42d>_42e)){
loc=dojo.hostenv.getBaseScriptUri()+loc;
}
return new dojo.uri.Uri(loc,uri);
};
this.Uri=function(){
var uri=arguments[0];
for(var i=1;i<arguments.length;i++){
if(!arguments[i]){
continue;
}
var _431=new dojo.uri.Uri(arguments[i].toString());
var _432=new dojo.uri.Uri(uri.toString());
if((_431.path=="")&&(_431.scheme==null)&&(_431.authority==null)&&(_431.query==null)){
if(_431.fragment!=null){
_432.fragment=_431.fragment;
}
_431=_432;
}else{
if(_431.scheme==null){
_431.scheme=_432.scheme;
if(_431.authority==null){
_431.authority=_432.authority;
if(_431.path.charAt(0)!="/"){
var path=_432.path.substring(0,_432.path.lastIndexOf("/")+1)+_431.path;
var segs=path.split("/");
for(var j=0;j<segs.length;j++){
if(segs[j]=="."){
if(j==segs.length-1){
segs[j]="";
}else{
segs.splice(j,1);
j--;
}
}else{
if(j>0&&!(j==1&&segs[0]=="")&&segs[j]==".."&&segs[j-1]!=".."){
if(j==segs.length-1){
segs.splice(j,1);
segs[j-1]="";
}else{
segs.splice(j-1,2);
j-=2;
}
}
}
}
_431.path=segs.join("/");
}
}
}
}
uri="";
if(_431.scheme!=null){
uri+=_431.scheme+":";
}
if(_431.authority!=null){
uri+="//"+_431.authority;
}
uri+=_431.path;
if(_431.query!=null){
uri+="?"+_431.query;
}
if(_431.fragment!=null){
uri+="#"+_431.fragment;
}
}
this.uri=uri.toString();
var _436="^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\\?([^#]*))?(#(.*))?$";
var r=this.uri.match(new RegExp(_436));
this.scheme=r[2]||(r[1]?"":null);
this.authority=r[4]||(r[3]?"":null);
this.path=r[5];
this.query=r[7]||(r[6]?"":null);
this.fragment=r[9]||(r[8]?"":null);
if(this.authority!=null){
_436="^((([^:]+:)?([^@]+))@)?([^:]*)(:([0-9]+))?$";
r=this.authority.match(new RegExp(_436));
this.user=r[3]||null;
this.password=r[4]||null;
this.host=r[5];
this.port=r[7]||null;
}
this.toString=function(){
return this.uri;
};
};
};
dojo.provide("dojo.html.style");
dojo.html.getClass=function(node){
node=dojo.byId(node);
if(!node){
return "";
}
var cs="";
if(node.className){
cs=node.className;
}else{
if(dojo.html.hasAttribute(node,"class")){
cs=dojo.html.getAttribute(node,"class");
}
}
return cs.replace(/^\s+|\s+$/g,"");
};
dojo.html.getClasses=function(node){
var c=dojo.html.getClass(node);
return (c=="")?[]:c.split(/\s+/g);
};
dojo.html.hasClass=function(node,_43d){
return (new RegExp("(^|\\s+)"+_43d+"(\\s+|$)")).test(dojo.html.getClass(node));
};
dojo.html.prependClass=function(node,_43f){
_43f+=" "+dojo.html.getClass(node);
return dojo.html.setClass(node,_43f);
};
dojo.html.addClass=function(node,_441){
if(dojo.html.hasClass(node,_441)){
return false;
}
_441=(dojo.html.getClass(node)+" "+_441).replace(/^\s+|\s+$/g,"");
return dojo.html.setClass(node,_441);
};
dojo.html.setClass=function(node,_443){
node=dojo.byId(node);
var cs=new String(_443);
try{
if(typeof node.className=="string"){
node.className=cs;
}else{
if(node.setAttribute){
node.setAttribute("class",_443);
node.className=cs;
}else{
return false;
}
}
}
catch(e){
dojo.debug("dojo.html.setClass() failed",e);
}
return true;
};
dojo.html.removeClass=function(node,_446,_447){
try{
if(!_447){
var _448=dojo.html.getClass(node).replace(new RegExp("(^|\\s+)"+_446+"(\\s+|$)"),"$1$2");
}else{
var _448=dojo.html.getClass(node).replace(_446,"");
}
dojo.html.setClass(node,_448);
}
catch(e){
dojo.debug("dojo.html.removeClass() failed",e);
}
return true;
};
dojo.html.replaceClass=function(node,_44a,_44b){
dojo.html.removeClass(node,_44b);
dojo.html.addClass(node,_44a);
};
dojo.html.classMatchType={ContainsAll:0,ContainsAny:1,IsOnly:2};
dojo.html.getElementsByClass=function(_44c,_44d,_44e,_44f,_450){
_450=false;
var _451=dojo.doc();
_44d=dojo.byId(_44d)||_451;
var _452=_44c.split(/\s+/g);
var _453=[];
if(_44f!=1&&_44f!=2){
_44f=0;
}
var _454=new RegExp("(\\s|^)(("+_452.join(")|(")+"))(\\s|$)");
var _455=_452.join(" ").length;
var _456=[];
if(!_450&&_451.evaluate){
var _457=".//"+(_44e||"*")+"[contains(";
if(_44f!=dojo.html.classMatchType.ContainsAny){
_457+="concat(' ',@class,' '), ' "+_452.join(" ') and contains(concat(' ',@class,' '), ' ")+" ')";
if(_44f==2){
_457+=" and string-length(@class)="+_455+"]";
}else{
_457+="]";
}
}else{
_457+="concat(' ',@class,' '), ' "+_452.join(" ') or contains(concat(' ',@class,' '), ' ")+" ')]";
}
var _458=_451.evaluate(_457,_44d,null,XPathResult.ANY_TYPE,null);
var _459=_458.iterateNext();
while(_459){
try{
_456.push(_459);
_459=_458.iterateNext();
}
catch(e){
break;
}
}
return _456;
}else{
if(!_44e){
_44e="*";
}
_456=_44d.getElementsByTagName(_44e);
var node,i=0;
outer:
while(node=_456[i++]){
var _45c=dojo.html.getClasses(node);
if(_45c.length==0){
continue outer;
}
var _45d=0;
for(var j=0;j<_45c.length;j++){
if(_454.test(_45c[j])){
if(_44f==dojo.html.classMatchType.ContainsAny){
_453.push(node);
continue outer;
}else{
_45d++;
}
}else{
if(_44f==dojo.html.classMatchType.IsOnly){
continue outer;
}
}
}
if(_45d==_452.length){
if((_44f==dojo.html.classMatchType.IsOnly)&&(_45d==_45c.length)){
_453.push(node);
}else{
if(_44f==dojo.html.classMatchType.ContainsAll){
_453.push(node);
}
}
}
}
return _453;
}
};
dojo.html.getElementsByClassName=dojo.html.getElementsByClass;
dojo.html.toCamelCase=function(_45f){
var arr=_45f.split("-"),cc=arr[0];
for(var i=1;i<arr.length;i++){
cc+=arr[i].charAt(0).toUpperCase()+arr[i].substring(1);
}
return cc;
};
dojo.html.toSelectorCase=function(_463){
return _463.replace(/([A-Z])/g,"-$1").toLowerCase();
};
if(dojo.render.html.ie){
dojo.html.getComputedStyle=function(node,_465,_466){
node=dojo.byId(node);
if(!node||!node.style){
return _466;
}
return node.currentStyle[dojo.html.toCamelCase(_465)];
};
dojo.html.getComputedStyles=function(node){
return node.currentStyle;
};
}else{
dojo.html.getComputedStyle=function(node,_469,_46a){
node=dojo.byId(node);
if(!node||!node.style){
return _46a;
}
var s=document.defaultView.getComputedStyle(node,null);
return (s&&s[dojo.html.toCamelCase(_469)])||"";
};
dojo.html.getComputedStyles=function(node){
return document.defaultView.getComputedStyle(node,null);
};
}
dojo.html.getStyleProperty=function(node,_46e){
node=dojo.byId(node);
return (node&&node.style?node.style[dojo.html.toCamelCase(_46e)]:undefined);
};
dojo.html.getStyle=function(node,_470){
var _471=dojo.html.getStyleProperty(node,_470);
return (_471?_471:dojo.html.getComputedStyle(node,_470));
};
dojo.html.setStyle=function(node,_473,_474){
node=dojo.byId(node);
if(node&&node.style){
var _475=dojo.html.toCamelCase(_473);
node.style[_475]=_474;
}
};
dojo.html.setStyleText=function(_476,text){
try{
_476.style.cssText=text;
}
catch(e){
_476.setAttribute("style",text);
}
};
dojo.html.copyStyle=function(_478,_479){
if(!_479.style.cssText){
_478.setAttribute("style",_479.getAttribute("style"));
}else{
_478.style.cssText=_479.style.cssText;
}
dojo.html.addClass(_478,dojo.html.getClass(_479));
};
dojo.html.getUnitValue=function(node,_47b,_47c){
var s=dojo.html.getComputedStyle(node,_47b);
if((!s)||((s=="auto")&&(_47c))){
return {value:0,units:"px"};
}
var _47e=s.match(/(\-?[\d.]+)([a-z%]*)/i);
if(!_47e){
return dojo.html.getUnitValue.bad;
}
return {value:Number(_47e[1]),units:_47e[2].toLowerCase()};
};
dojo.html.getUnitValue.bad={value:NaN,units:""};
if(dojo.render.html.ie){
dojo.html.toPixelValue=function(_47f,_480){
if(!_480){
return 0;
}
if(_480.slice(-2)=="px"){
return parseFloat(_480);
}
var _481=0;
with(_47f){
var _482=style.left;
var _483=runtimeStyle.left;
runtimeStyle.left=currentStyle.left;
try{
style.left=_480||0;
_481=style.pixelLeft;
style.left=_482;
runtimeStyle.left=_483;
}
catch(e){
}
}
return _481;
};
}else{
dojo.html.toPixelValue=function(_484,_485){
return (_485&&(_485.slice(-2)=="px")?parseFloat(_485):0);
};
}
dojo.html.getPixelValue=function(node,_487,_488){
return dojo.html.toPixelValue(node,dojo.html.getComputedStyle(node,_487));
};
dojo.html.setPositivePixelValue=function(node,_48a,_48b){
if(isNaN(_48b)){
return false;
}
node.style[_48a]=Math.max(0,_48b)+"px";
return true;
};
dojo.html.styleSheet=null;
dojo.html.insertCssRule=function(_48c,_48d,_48e){
if(!dojo.html.styleSheet){
if(document.createStyleSheet){
dojo.html.styleSheet=document.createStyleSheet();
}else{
if(document.styleSheets[0]){
dojo.html.styleSheet=document.styleSheets[0];
}else{
return null;
}
}
}
if(arguments.length<3){
if(dojo.html.styleSheet.cssRules){
_48e=dojo.html.styleSheet.cssRules.length;
}else{
if(dojo.html.styleSheet.rules){
_48e=dojo.html.styleSheet.rules.length;
}else{
return null;
}
}
}
if(dojo.html.styleSheet.insertRule){
var rule=_48c+" { "+_48d+" }";
return dojo.html.styleSheet.insertRule(rule,_48e);
}else{
if(dojo.html.styleSheet.addRule){
return dojo.html.styleSheet.addRule(_48c,_48d,_48e);
}else{
return null;
}
}
};
dojo.html.removeCssRule=function(_490){
if(!dojo.html.styleSheet){
dojo.debug("no stylesheet defined for removing rules");
return false;
}
if(dojo.render.html.ie){
if(!_490){
_490=dojo.html.styleSheet.rules.length;
dojo.html.styleSheet.removeRule(_490);
}
}else{
if(document.styleSheets[0]){
if(!_490){
_490=dojo.html.styleSheet.cssRules.length;
}
dojo.html.styleSheet.deleteRule(_490);
}
}
return true;
};
dojo.html._insertedCssFiles=[];
dojo.html.insertCssFile=function(URI,doc,_493,_494){
if(!URI){
return;
}
if(!doc){
doc=document;
}
var _495=dojo.hostenv.getText(URI,false,_494);
if(_495===null){
return;
}
_495=dojo.html.fixPathsInCssText(_495,URI);
if(_493){
var idx=-1,node,ent=dojo.html._insertedCssFiles;
for(var i=0;i<ent.length;i++){
if((ent[i].doc==doc)&&(ent[i].cssText==_495)){
idx=i;
node=ent[i].nodeRef;
break;
}
}
if(node){
var _49a=doc.getElementsByTagName("style");
for(var i=0;i<_49a.length;i++){
if(_49a[i]==node){
return;
}
}
dojo.html._insertedCssFiles.shift(idx,1);
}
}
var _49b=dojo.html.insertCssText(_495,doc);
dojo.html._insertedCssFiles.push({"doc":doc,"cssText":_495,"nodeRef":_49b});
if(_49b&&djConfig.isDebug){
_49b.setAttribute("dbgHref",URI);
}
return _49b;
};
dojo.html.insertCssText=function(_49c,doc,URI){
if(!_49c){
return;
}
if(!doc){
doc=document;
}
if(URI){
_49c=dojo.html.fixPathsInCssText(_49c,URI);
}
var _49f=doc.createElement("style");
_49f.setAttribute("type","text/css");
var head=doc.getElementsByTagName("head")[0];
if(!head){
dojo.debug("No head tag in document, aborting styles");
return;
}else{
head.appendChild(_49f);
}
if(_49f.styleSheet){
var _4a1=function(){
try{
_49f.styleSheet.cssText=_49c;
}
catch(e){
dojo.debug(e);
}
};
if(_49f.styleSheet.disabled){
setTimeout(_4a1,10);
}else{
_4a1();
}
}else{
var _4a2=doc.createTextNode(_49c);
_49f.appendChild(_4a2);
}
return _49f;
};
dojo.html.fixPathsInCssText=function(_4a3,URI){
if(!_4a3||!URI){
return;
}
var _4a5,str="",url="",_4a8="[\\t\\s\\w\\(\\)\\/\\.\\\\'\"-:#=&?~]+";
var _4a9=new RegExp("url\\(\\s*("+_4a8+")\\s*\\)");
var _4aa=/(file|https?|ftps?):\/\//;
regexTrim=new RegExp("^[\\s]*(['\"]?)("+_4a8+")\\1[\\s]*?$");
if(dojo.render.html.ie55||dojo.render.html.ie60){
var _4ab=new RegExp("AlphaImageLoader\\((.*)src=['\"]("+_4a8+")['\"]");
while(_4a5=_4ab.exec(_4a3)){
url=_4a5[2].replace(regexTrim,"$2");
if(!_4aa.exec(url)){
url=(new dojo.uri.Uri(URI,url).toString());
}
str+=_4a3.substring(0,_4a5.index)+"AlphaImageLoader("+_4a5[1]+"src='"+url+"'";
_4a3=_4a3.substr(_4a5.index+_4a5[0].length);
}
_4a3=str+_4a3;
str="";
}
while(_4a5=_4a9.exec(_4a3)){
url=_4a5[1].replace(regexTrim,"$2");
if(!_4aa.exec(url)){
url=(new dojo.uri.Uri(URI,url).toString());
}
str+=_4a3.substring(0,_4a5.index)+"url("+url+")";
_4a3=_4a3.substr(_4a5.index+_4a5[0].length);
}
return str+_4a3;
};
dojo.html.setActiveStyleSheet=function(_4ac){
var i=0,a,els=dojo.doc().getElementsByTagName("link");
while(a=els[i++]){
if(a.getAttribute("rel").indexOf("style")!=-1&&a.getAttribute("title")){
a.disabled=true;
if(a.getAttribute("title")==_4ac){
a.disabled=false;
}
}
}
};
dojo.html.getActiveStyleSheet=function(){
var i=0,a,els=dojo.doc().getElementsByTagName("link");
while(a=els[i++]){
if(a.getAttribute("rel").indexOf("style")!=-1&&a.getAttribute("title")&&!a.disabled){
return a.getAttribute("title");
}
}
return null;
};
dojo.html.getPreferredStyleSheet=function(){
var i=0,a,els=dojo.doc().getElementsByTagName("link");
while(a=els[i++]){
if(a.getAttribute("rel").indexOf("style")!=-1&&a.getAttribute("rel").indexOf("alt")==-1&&a.getAttribute("title")){
return a.getAttribute("title");
}
}
return null;
};
dojo.html.applyBrowserClass=function(node){
var drh=dojo.render.html;
var _4b8={dj_ie:drh.ie,dj_ie55:drh.ie55,dj_ie6:drh.ie60,dj_ie7:drh.ie70,dj_iequirks:drh.ie&&drh.quirks,dj_opera:drh.opera,dj_opera8:drh.opera&&(Math.floor(dojo.render.version)==8),dj_opera9:drh.opera&&(Math.floor(dojo.render.version)==9),dj_khtml:drh.khtml,dj_safari:drh.safari,dj_gecko:drh.mozilla};
for(var p in _4b8){
if(_4b8[p]){
dojo.html.addClass(node,p);
}
}
};
dojo.provide("dojo.html.display");
dojo.html._toggle=function(node,_4bb,_4bc){
node=dojo.byId(node);
_4bc(node,!_4bb(node));
return _4bb(node);
};
dojo.html.show=function(node){
node=dojo.byId(node);
if(dojo.html.getStyleProperty(node,"display")=="none"){
dojo.html.setStyle(node,"display",(node.dojoDisplayCache||""));
node.dojoDisplayCache=undefined;
}
};
dojo.html.hide=function(node){
node=dojo.byId(node);
if(typeof node["dojoDisplayCache"]=="undefined"){
var d=dojo.html.getStyleProperty(node,"display");
if(d!="none"){
node.dojoDisplayCache=d;
}
}
dojo.html.setStyle(node,"display","none");
};
dojo.html.setShowing=function(node,_4c1){
dojo.html[(_4c1?"show":"hide")](node);
};
dojo.html.isShowing=function(node){
return (dojo.html.getStyleProperty(node,"display")!="none");
};
dojo.html.toggleShowing=function(node){
return dojo.html._toggle(node,dojo.html.isShowing,dojo.html.setShowing);
};
dojo.html.displayMap={tr:"",td:"",th:"",img:"inline",span:"inline",input:"inline",button:"inline"};
dojo.html.suggestDisplayByTagName=function(node){
node=dojo.byId(node);
if(node&&node.tagName){
var tag=node.tagName.toLowerCase();
return (tag in dojo.html.displayMap?dojo.html.displayMap[tag]:"block");
}
};
dojo.html.setDisplay=function(node,_4c7){
dojo.html.setStyle(node,"display",((_4c7 instanceof String||typeof _4c7=="string")?_4c7:(_4c7?dojo.html.suggestDisplayByTagName(node):"none")));
};
dojo.html.isDisplayed=function(node){
return (dojo.html.getComputedStyle(node,"display")!="none");
};
dojo.html.toggleDisplay=function(node){
return dojo.html._toggle(node,dojo.html.isDisplayed,dojo.html.setDisplay);
};
dojo.html.setVisibility=function(node,_4cb){
dojo.html.setStyle(node,"visibility",((_4cb instanceof String||typeof _4cb=="string")?_4cb:(_4cb?"visible":"hidden")));
};
dojo.html.isVisible=function(node){
return (dojo.html.getComputedStyle(node,"visibility")!="hidden");
};
dojo.html.toggleVisibility=function(node){
return dojo.html._toggle(node,dojo.html.isVisible,dojo.html.setVisibility);
};
dojo.html.setOpacity=function(node,_4cf,_4d0){
node=dojo.byId(node);
var h=dojo.render.html;
if(!_4d0){
if(_4cf>=1){
if(h.ie){
dojo.html.clearOpacity(node);
return;
}else{
_4cf=0.999999;
}
}else{
if(_4cf<0){
_4cf=0;
}
}
}
if(h.ie){
if(node.nodeName.toLowerCase()=="tr"){
var tds=node.getElementsByTagName("td");
for(var x=0;x<tds.length;x++){
tds[x].style.filter="Alpha(Opacity="+_4cf*100+")";
}
}
node.style.filter="Alpha(Opacity="+_4cf*100+")";
}else{
if(h.moz){
node.style.opacity=_4cf;
node.style.MozOpacity=_4cf;
}else{
if(h.safari){
node.style.opacity=_4cf;
node.style.KhtmlOpacity=_4cf;
}else{
node.style.opacity=_4cf;
}
}
}
};
dojo.html.clearOpacity=function(node){
node=dojo.byId(node);
var ns=node.style;
var h=dojo.render.html;
if(h.ie){
try{
if(node.filters&&node.filters.alpha){
ns.filter="";
}
}
catch(e){
}
}else{
if(h.moz){
ns.opacity=1;
ns.MozOpacity=1;
}else{
if(h.safari){
ns.opacity=1;
ns.KhtmlOpacity=1;
}else{
ns.opacity=1;
}
}
}
};
dojo.html.getOpacity=function(node){
node=dojo.byId(node);
var h=dojo.render.html;
if(h.ie){
var opac=(node.filters&&node.filters.alpha&&typeof node.filters.alpha.opacity=="number"?node.filters.alpha.opacity:100)/100;
}else{
var opac=node.style.opacity||node.style.MozOpacity||node.style.KhtmlOpacity||1;
}
return opac>=0.999999?1:Number(opac);
};
dojo.provide("dojo.html.color");
dojo.html.getBackgroundColor=function(node){
node=dojo.byId(node);
var _4db;
do{
_4db=dojo.html.getStyle(node,"background-color");
if(_4db.toLowerCase()=="rgba(0, 0, 0, 0)"){
_4db="transparent";
}
if(node==document.getElementsByTagName("body")[0]){
node=null;
break;
}
node=node.parentNode;
}while(node&&dojo.lang.inArray(["transparent",""],_4db));
if(_4db=="transparent"){
_4db=[255,255,255,0];
}else{
_4db=dojo.gfx.color.extractRGB(_4db);
}
return _4db;
};
dojo.provide("dojo.html.layout");
dojo.html.sumAncestorProperties=function(node,prop){
node=dojo.byId(node);
if(!node){
return 0;
}
var _4de=0;
while(node){
if(dojo.html.getComputedStyle(node,"position")=="fixed"){
return 0;
}
var val=node[prop];
if(val){
_4de+=val-0;
if(node==dojo.body()){
break;
}
}
node=node.parentNode;
}
return _4de;
};
dojo.html.setStyleAttributes=function(node,_4e1){
node=dojo.byId(node);
var _4e2=_4e1.replace(/(;)?\s*$/,"").split(";");
for(var i=0;i<_4e2.length;i++){
var _4e4=_4e2[i].split(":");
var name=_4e4[0].replace(/\s*$/,"").replace(/^\s*/,"").toLowerCase();
var _4e6=_4e4[1].replace(/\s*$/,"").replace(/^\s*/,"");
switch(name){
case "opacity":
dojo.html.setOpacity(node,_4e6);
break;
case "content-height":
dojo.html.setContentBox(node,{height:_4e6});
break;
case "content-width":
dojo.html.setContentBox(node,{width:_4e6});
break;
case "outer-height":
dojo.html.setMarginBox(node,{height:_4e6});
break;
case "outer-width":
dojo.html.setMarginBox(node,{width:_4e6});
break;
default:
node.style[dojo.html.toCamelCase(name)]=_4e6;
}
}
};
dojo.html.boxSizing={MARGIN_BOX:"margin-box",BORDER_BOX:"border-box",PADDING_BOX:"padding-box",CONTENT_BOX:"content-box"};
dojo.html.getAbsolutePosition=dojo.html.abs=function(node,_4e8,_4e9){
node=dojo.byId(node,node.ownerDocument);
var ret={x:0,y:0};
var bs=dojo.html.boxSizing;
if(!_4e9){
_4e9=bs.CONTENT_BOX;
}
var _4ec=2;
var _4ed;
switch(_4e9){
case bs.MARGIN_BOX:
_4ed=3;
break;
case bs.BORDER_BOX:
_4ed=2;
break;
case bs.PADDING_BOX:
default:
_4ed=1;
break;
case bs.CONTENT_BOX:
_4ed=0;
break;
}
var h=dojo.render.html;
var db=document["body"]||document["documentElement"];
if(h.ie){
with(node.getBoundingClientRect()){
ret.x=left-2;
ret.y=top-2;
}
}else{
if(document.getBoxObjectFor){
_4ec=1;
try{
var bo=document.getBoxObjectFor(node);
ret.x=bo.x-dojo.html.sumAncestorProperties(node,"scrollLeft");
ret.y=bo.y-dojo.html.sumAncestorProperties(node,"scrollTop");
}
catch(e){
}
}else{
if(node["offsetParent"]){
var _4f1;
if((h.safari)&&(node.style.getPropertyValue("position")=="absolute")&&(node.parentNode==db)){
_4f1=db;
}else{
_4f1=db.parentNode;
}
if(node.parentNode!=db){
var nd=node;
if(dojo.render.html.opera){
nd=db;
}
ret.x-=dojo.html.sumAncestorProperties(nd,"scrollLeft");
ret.y-=dojo.html.sumAncestorProperties(nd,"scrollTop");
}
var _4f3=node;
do{
var n=_4f3["offsetLeft"];
if(!h.opera||n>0){
ret.x+=isNaN(n)?0:n;
}
var m=_4f3["offsetTop"];
ret.y+=isNaN(m)?0:m;
_4f3=_4f3.offsetParent;
}while((_4f3!=_4f1)&&(_4f3!=null));
}else{
if(node["x"]&&node["y"]){
ret.x+=isNaN(node.x)?0:node.x;
ret.y+=isNaN(node.y)?0:node.y;
}
}
}
}
if(_4e8){
var _4f6=dojo.html.getScroll();
ret.y+=_4f6.top;
ret.x+=_4f6.left;
}
var _4f7=[dojo.html.getPaddingExtent,dojo.html.getBorderExtent,dojo.html.getMarginExtent];
if(_4ec>_4ed){
for(var i=_4ed;i<_4ec;++i){
ret.y+=_4f7[i](node,"top");
ret.x+=_4f7[i](node,"left");
}
}else{
if(_4ec<_4ed){
for(var i=_4ed;i>_4ec;--i){
ret.y-=_4f7[i-1](node,"top");
ret.x-=_4f7[i-1](node,"left");
}
}
}
ret.top=ret.y;
ret.left=ret.x;
return ret;
};
dojo.html.isPositionAbsolute=function(node){
return (dojo.html.getComputedStyle(node,"position")=="absolute");
};
dojo.html._sumPixelValues=function(node,_4fb,_4fc){
var _4fd=0;
for(var x=0;x<_4fb.length;x++){
_4fd+=dojo.html.getPixelValue(node,_4fb[x],_4fc);
}
return _4fd;
};
dojo.html.getMargin=function(node){
return {width:dojo.html._sumPixelValues(node,["margin-left","margin-right"],(dojo.html.getComputedStyle(node,"position")=="absolute")),height:dojo.html._sumPixelValues(node,["margin-top","margin-bottom"],(dojo.html.getComputedStyle(node,"position")=="absolute"))};
};
dojo.html.getBorder=function(node){
return {width:dojo.html.getBorderExtent(node,"left")+dojo.html.getBorderExtent(node,"right"),height:dojo.html.getBorderExtent(node,"top")+dojo.html.getBorderExtent(node,"bottom")};
};
dojo.html.getBorderExtent=function(node,side){
return (dojo.html.getStyle(node,"border-"+side+"-style")=="none"?0:dojo.html.getPixelValue(node,"border-"+side+"-width"));
};
dojo.html.getMarginExtent=function(node,side){
return dojo.html._sumPixelValues(node,["margin-"+side],dojo.html.isPositionAbsolute(node));
};
dojo.html.getPaddingExtent=function(node,side){
return dojo.html._sumPixelValues(node,["padding-"+side],true);
};
dojo.html.getPadding=function(node){
return {width:dojo.html._sumPixelValues(node,["padding-left","padding-right"],true),height:dojo.html._sumPixelValues(node,["padding-top","padding-bottom"],true)};
};
dojo.html.getPadBorder=function(node){
var pad=dojo.html.getPadding(node);
var _50a=dojo.html.getBorder(node);
return {width:pad.width+_50a.width,height:pad.height+_50a.height};
};
dojo.html.getBoxSizing=function(node){
var h=dojo.render.html;
var bs=dojo.html.boxSizing;
if(((h.ie)||(h.opera))&&node.nodeName.toLowerCase()!="img"){
var cm=document["compatMode"];
if((cm=="BackCompat")||(cm=="QuirksMode")){
return bs.BORDER_BOX;
}else{
return bs.CONTENT_BOX;
}
}else{
if(arguments.length==0){
node=document.documentElement;
}
var _50f;
if(!h.ie){
_50f=dojo.html.getStyle(node,"-moz-box-sizing");
if(!_50f){
_50f=dojo.html.getStyle(node,"box-sizing");
}
}
return (_50f?_50f:bs.CONTENT_BOX);
}
};
dojo.html.isBorderBox=function(node){
return (dojo.html.getBoxSizing(node)==dojo.html.boxSizing.BORDER_BOX);
};
dojo.html.getBorderBox=function(node){
node=dojo.byId(node);
return {width:node.offsetWidth,height:node.offsetHeight};
};
dojo.html.getPaddingBox=function(node){
var box=dojo.html.getBorderBox(node);
var _514=dojo.html.getBorder(node);
return {width:box.width-_514.width,height:box.height-_514.height};
};
dojo.html.getContentBox=function(node){
node=dojo.byId(node);
var _516=dojo.html.getPadBorder(node);
return {width:node.offsetWidth-_516.width,height:node.offsetHeight-_516.height};
};
dojo.html.setContentBox=function(node,args){
node=dojo.byId(node);
var _519=0;
var _51a=0;
var isbb=dojo.html.isBorderBox(node);
var _51c=(isbb?dojo.html.getPadBorder(node):{width:0,height:0});
var ret={};
if(typeof args.width!="undefined"){
_519=args.width+_51c.width;
ret.width=dojo.html.setPositivePixelValue(node,"width",_519);
}
if(typeof args.height!="undefined"){
_51a=args.height+_51c.height;
ret.height=dojo.html.setPositivePixelValue(node,"height",_51a);
}
return ret;
};
dojo.html.getMarginBox=function(node){
var _51f=dojo.html.getBorderBox(node);
var _520=dojo.html.getMargin(node);
return {width:_51f.width+_520.width,height:_51f.height+_520.height};
};
dojo.html.setMarginBox=function(node,args){
node=dojo.byId(node);
var _523=0;
var _524=0;
var isbb=dojo.html.isBorderBox(node);
var _526=(!isbb?dojo.html.getPadBorder(node):{width:0,height:0});
var _527=dojo.html.getMargin(node);
var ret={};
if(typeof args.width!="undefined"){
_523=args.width-_526.width;
_523-=_527.width;
ret.width=dojo.html.setPositivePixelValue(node,"width",_523);
}
if(typeof args.height!="undefined"){
_524=args.height-_526.height;
_524-=_527.height;
ret.height=dojo.html.setPositivePixelValue(node,"height",_524);
}
return ret;
};
dojo.html.getElementBox=function(node,type){
var bs=dojo.html.boxSizing;
switch(type){
case bs.MARGIN_BOX:
return dojo.html.getMarginBox(node);
case bs.BORDER_BOX:
return dojo.html.getBorderBox(node);
case bs.PADDING_BOX:
return dojo.html.getPaddingBox(node);
case bs.CONTENT_BOX:
default:
return dojo.html.getContentBox(node);
}
};
dojo.html.toCoordinateObject=dojo.html.toCoordinateArray=function(_52c,_52d,_52e){
if(_52c instanceof Array||typeof _52c=="array"){
dojo.deprecated("dojo.html.toCoordinateArray","use dojo.html.toCoordinateObject({left: , top: , width: , height: }) instead","0.5");
while(_52c.length<4){
_52c.push(0);
}
while(_52c.length>4){
_52c.pop();
}
var ret={left:_52c[0],top:_52c[1],width:_52c[2],height:_52c[3]};
}else{
if(!_52c.nodeType&&!(_52c instanceof String||typeof _52c=="string")&&("width" in _52c||"height" in _52c||"left" in _52c||"x" in _52c||"top" in _52c||"y" in _52c)){
var ret={left:_52c.left||_52c.x||0,top:_52c.top||_52c.y||0,width:_52c.width||0,height:_52c.height||0};
}else{
var node=dojo.byId(_52c);
var pos=dojo.html.abs(node,_52d,_52e);
var _532=dojo.html.getMarginBox(node);
var ret={left:pos.left,top:pos.top,width:_532.width,height:_532.height};
}
}
ret.x=ret.left;
ret.y=ret.top;
return ret;
};
dojo.html.setMarginBoxWidth=dojo.html.setOuterWidth=function(node,_534){
return dojo.html._callDeprecated("setMarginBoxWidth","setMarginBox",arguments,"width");
};
dojo.html.setMarginBoxHeight=dojo.html.setOuterHeight=function(){
return dojo.html._callDeprecated("setMarginBoxHeight","setMarginBox",arguments,"height");
};
dojo.html.getMarginBoxWidth=dojo.html.getOuterWidth=function(){
return dojo.html._callDeprecated("getMarginBoxWidth","getMarginBox",arguments,null,"width");
};
dojo.html.getMarginBoxHeight=dojo.html.getOuterHeight=function(){
return dojo.html._callDeprecated("getMarginBoxHeight","getMarginBox",arguments,null,"height");
};
dojo.html.getTotalOffset=function(node,type,_537){
return dojo.html._callDeprecated("getTotalOffset","getAbsolutePosition",arguments,null,type);
};
dojo.html.getAbsoluteX=function(node,_539){
return dojo.html._callDeprecated("getAbsoluteX","getAbsolutePosition",arguments,null,"x");
};
dojo.html.getAbsoluteY=function(node,_53b){
return dojo.html._callDeprecated("getAbsoluteY","getAbsolutePosition",arguments,null,"y");
};
dojo.html.totalOffsetLeft=function(node,_53d){
return dojo.html._callDeprecated("totalOffsetLeft","getAbsolutePosition",arguments,null,"left");
};
dojo.html.totalOffsetTop=function(node,_53f){
return dojo.html._callDeprecated("totalOffsetTop","getAbsolutePosition",arguments,null,"top");
};
dojo.html.getMarginWidth=function(node){
return dojo.html._callDeprecated("getMarginWidth","getMargin",arguments,null,"width");
};
dojo.html.getMarginHeight=function(node){
return dojo.html._callDeprecated("getMarginHeight","getMargin",arguments,null,"height");
};
dojo.html.getBorderWidth=function(node){
return dojo.html._callDeprecated("getBorderWidth","getBorder",arguments,null,"width");
};
dojo.html.getBorderHeight=function(node){
return dojo.html._callDeprecated("getBorderHeight","getBorder",arguments,null,"height");
};
dojo.html.getPaddingWidth=function(node){
return dojo.html._callDeprecated("getPaddingWidth","getPadding",arguments,null,"width");
};
dojo.html.getPaddingHeight=function(node){
return dojo.html._callDeprecated("getPaddingHeight","getPadding",arguments,null,"height");
};
dojo.html.getPadBorderWidth=function(node){
return dojo.html._callDeprecated("getPadBorderWidth","getPadBorder",arguments,null,"width");
};
dojo.html.getPadBorderHeight=function(node){
return dojo.html._callDeprecated("getPadBorderHeight","getPadBorder",arguments,null,"height");
};
dojo.html.getBorderBoxWidth=dojo.html.getInnerWidth=function(){
return dojo.html._callDeprecated("getBorderBoxWidth","getBorderBox",arguments,null,"width");
};
dojo.html.getBorderBoxHeight=dojo.html.getInnerHeight=function(){
return dojo.html._callDeprecated("getBorderBoxHeight","getBorderBox",arguments,null,"height");
};
dojo.html.getContentBoxWidth=dojo.html.getContentWidth=function(){
return dojo.html._callDeprecated("getContentBoxWidth","getContentBox",arguments,null,"width");
};
dojo.html.getContentBoxHeight=dojo.html.getContentHeight=function(){
return dojo.html._callDeprecated("getContentBoxHeight","getContentBox",arguments,null,"height");
};
dojo.html.setContentBoxWidth=dojo.html.setContentWidth=function(node,_549){
return dojo.html._callDeprecated("setContentBoxWidth","setContentBox",arguments,"width");
};
dojo.html.setContentBoxHeight=dojo.html.setContentHeight=function(node,_54b){
return dojo.html._callDeprecated("setContentBoxHeight","setContentBox",arguments,"height");
};
dojo.provide("dojo.lfx.html");
dojo.lfx.html._byId=function(_54c){
if(!_54c){
return [];
}
if(dojo.lang.isArrayLike(_54c)){
if(!_54c.alreadyChecked){
var n=[];
dojo.lang.forEach(_54c,function(node){
n.push(dojo.byId(node));
});
n.alreadyChecked=true;
return n;
}else{
return _54c;
}
}else{
var n=[];
n.push(dojo.byId(_54c));
n.alreadyChecked=true;
return n;
}
};
dojo.lfx.html.propertyAnimation=function(_54f,_550,_551,_552,_553){
_54f=dojo.lfx.html._byId(_54f);
var _554={"propertyMap":_550,"nodes":_54f,"duration":_551,"easing":_552||dojo.lfx.easeDefault};
var _555=function(args){
if(args.nodes.length==1){
var pm=args.propertyMap;
if(!dojo.lang.isArray(args.propertyMap)){
var parr=[];
for(var _559 in pm){
pm[_559].property=_559;
parr.push(pm[_559]);
}
pm=args.propertyMap=parr;
}
dojo.lang.forEach(pm,function(prop){
if(dj_undef("start",prop)){
if(prop.property!="opacity"){
prop.start=parseInt(dojo.html.getComputedStyle(args.nodes[0],prop.property));
}else{
prop.start=dojo.html.getOpacity(args.nodes[0]);
}
}
});
}
};
var _55b=function(_55c){
var _55d=[];
dojo.lang.forEach(_55c,function(c){
_55d.push(Math.round(c));
});
return _55d;
};
var _55f=function(n,_561){
n=dojo.byId(n);
if(!n||!n.style){
return;
}
for(var s in _561){
try{
if(s=="opacity"){
dojo.html.setOpacity(n,_561[s]);
}else{
n.style[s]=_561[s];
}
}
catch(e){
dojo.debug(e);
}
}
};
var _563=function(_564){
this._properties=_564;
this.diffs=new Array(_564.length);
dojo.lang.forEach(_564,function(prop,i){
if(dojo.lang.isFunction(prop.start)){
prop.start=prop.start(prop,i);
}
if(dojo.lang.isFunction(prop.end)){
prop.end=prop.end(prop,i);
}
if(dojo.lang.isArray(prop.start)){
this.diffs[i]=null;
}else{
if(prop.start instanceof dojo.gfx.color.Color){
prop.startRgb=prop.start.toRgb();
prop.endRgb=prop.end.toRgb();
}else{
this.diffs[i]=prop.end-prop.start;
}
}
},this);
this.getValue=function(n){
var ret={};
dojo.lang.forEach(this._properties,function(prop,i){
var _56b=null;
if(dojo.lang.isArray(prop.start)){
}else{
if(prop.start instanceof dojo.gfx.color.Color){
_56b=(prop.units||"rgb")+"(";
for(var j=0;j<prop.startRgb.length;j++){
_56b+=Math.round(((prop.endRgb[j]-prop.startRgb[j])*n)+prop.startRgb[j])+(j<prop.startRgb.length-1?",":"");
}
_56b+=")";
}else{
_56b=((this.diffs[i])*n)+prop.start+(prop.property!="opacity"?prop.units||"px":"");
}
}
ret[dojo.html.toCamelCase(prop.property)]=_56b;
},this);
return ret;
};
};
var anim=new dojo.lfx.Animation({beforeBegin:function(){
_555(_554);
anim.curve=new _563(_554.propertyMap);
},onAnimate:function(_56e){
dojo.lang.forEach(_554.nodes,function(node){
_55f(node,_56e);
});
}},_554.duration,null,_554.easing);
if(_553){
for(var x in _553){
if(dojo.lang.isFunction(_553[x])){
anim.connect(x,anim,_553[x]);
}
}
}
return anim;
};
dojo.lfx.html._makeFadeable=function(_571){
var _572=function(node){
if(dojo.render.html.ie){
if((node.style.zoom.length==0)&&(dojo.html.getStyle(node,"zoom")=="normal")){
node.style.zoom="1";
}
if((node.style.width.length==0)&&(dojo.html.getStyle(node,"width")=="auto")){
node.style.width="auto";
}
}
};
if(dojo.lang.isArrayLike(_571)){
dojo.lang.forEach(_571,_572);
}else{
_572(_571);
}
};
dojo.lfx.html.fade=function(_574,_575,_576,_577,_578){
_574=dojo.lfx.html._byId(_574);
var _579={property:"opacity"};
if(!dj_undef("start",_575)){
_579.start=_575.start;
}else{
_579.start=function(){
return dojo.html.getOpacity(_574[0]);
};
}
if(!dj_undef("end",_575)){
_579.end=_575.end;
}else{
dojo.raise("dojo.lfx.html.fade needs an end value");
}
var anim=dojo.lfx.propertyAnimation(_574,[_579],_576,_577);
anim.connect("beforeBegin",function(){
dojo.lfx.html._makeFadeable(_574);
});
if(_578){
anim.connect("onEnd",function(){
_578(_574,anim);
});
}
return anim;
};
dojo.lfx.html.fadeIn=function(_57b,_57c,_57d,_57e){
return dojo.lfx.html.fade(_57b,{end:1},_57c,_57d,_57e);
};
dojo.lfx.html.fadeOut=function(_57f,_580,_581,_582){
return dojo.lfx.html.fade(_57f,{end:0},_580,_581,_582);
};
dojo.lfx.html.fadeShow=function(_583,_584,_585,_586){
_583=dojo.lfx.html._byId(_583);
dojo.lang.forEach(_583,function(node){
dojo.html.setOpacity(node,0);
});
var anim=dojo.lfx.html.fadeIn(_583,_584,_585,_586);
anim.connect("beforeBegin",function(){
if(dojo.lang.isArrayLike(_583)){
dojo.lang.forEach(_583,dojo.html.show);
}else{
dojo.html.show(_583);
}
});
return anim;
};
dojo.lfx.html.fadeHide=function(_589,_58a,_58b,_58c){
var anim=dojo.lfx.html.fadeOut(_589,_58a,_58b,function(){
if(dojo.lang.isArrayLike(_589)){
dojo.lang.forEach(_589,dojo.html.hide);
}else{
dojo.html.hide(_589);
}
if(_58c){
_58c(_589,anim);
}
});
return anim;
};
dojo.lfx.html.wipeIn=function(_58e,_58f,_590,_591){
_58e=dojo.lfx.html._byId(_58e);
var _592=[];
dojo.lang.forEach(_58e,function(node){
var _594={};
var _595,_596,_597;
with(node.style){
_595=top;
_596=left;
_597=position;
top="-9999px";
left="-9999px";
position="absolute";
display="";
}
var _598=dojo.html.getBorderBox(node).height;
with(node.style){
top=_595;
left=_596;
position=_597;
display="none";
}
var anim=dojo.lfx.propertyAnimation(node,{"height":{start:1,end:function(){
return _598;
}}},_58f,_590);
anim.connect("beforeBegin",function(){
_594.overflow=node.style.overflow;
_594.height=node.style.height;
with(node.style){
overflow="hidden";
height="1px";
}
dojo.html.show(node);
});
anim.connect("onEnd",function(){
with(node.style){
overflow=_594.overflow;
height=_594.height;
}
if(_591){
_591(node,anim);
}
});
_592.push(anim);
});
return dojo.lfx.combine(_592);
};
dojo.lfx.html.wipeOut=function(_59a,_59b,_59c,_59d){
_59a=dojo.lfx.html._byId(_59a);
var _59e=[];
dojo.lang.forEach(_59a,function(node){
var _5a0={};
var anim=dojo.lfx.propertyAnimation(node,{"height":{start:function(){
return dojo.html.getContentBox(node).height;
},end:1}},_59b,_59c,{"beforeBegin":function(){
_5a0.overflow=node.style.overflow;
_5a0.height=node.style.height;
with(node.style){
overflow="hidden";
}
dojo.html.show(node);
},"onEnd":function(){
dojo.html.hide(node);
with(node.style){
overflow=_5a0.overflow;
height=_5a0.height;
}
if(_59d){
_59d(node,anim);
}
}});
_59e.push(anim);
});
return dojo.lfx.combine(_59e);
};
dojo.lfx.html.slideTo=function(_5a2,_5a3,_5a4,_5a5,_5a6){
_5a2=dojo.lfx.html._byId(_5a2);
var _5a7=[];
var _5a8=dojo.html.getComputedStyle;
if(dojo.lang.isArray(_5a3)){
dojo.deprecated("dojo.lfx.html.slideTo(node, array)","use dojo.lfx.html.slideTo(node, {top: value, left: value});","0.5");
_5a3={top:_5a3[0],left:_5a3[1]};
}
dojo.lang.forEach(_5a2,function(node){
var top=null;
var left=null;
var init=(function(){
var _5ad=node;
return function(){
var pos=_5a8(_5ad,"position");
top=(pos=="absolute"?node.offsetTop:parseInt(_5a8(node,"top"))||0);
left=(pos=="absolute"?node.offsetLeft:parseInt(_5a8(node,"left"))||0);
if(!dojo.lang.inArray(["absolute","relative"],pos)){
var ret=dojo.html.abs(_5ad,true);
dojo.html.setStyleAttributes(_5ad,"position:absolute;top:"+ret.y+"px;left:"+ret.x+"px;");
top=ret.y;
left=ret.x;
}
};
})();
init();
var anim=dojo.lfx.propertyAnimation(node,{"top":{start:top,end:(_5a3.top||0)},"left":{start:left,end:(_5a3.left||0)}},_5a4,_5a5,{"beforeBegin":init});
if(_5a6){
anim.connect("onEnd",function(){
_5a6(_5a2,anim);
});
}
_5a7.push(anim);
});
return dojo.lfx.combine(_5a7);
};
dojo.lfx.html.slideBy=function(_5b1,_5b2,_5b3,_5b4,_5b5){
_5b1=dojo.lfx.html._byId(_5b1);
var _5b6=[];
var _5b7=dojo.html.getComputedStyle;
if(dojo.lang.isArray(_5b2)){
dojo.deprecated("dojo.lfx.html.slideBy(node, array)","use dojo.lfx.html.slideBy(node, {top: value, left: value});","0.5");
_5b2={top:_5b2[0],left:_5b2[1]};
}
dojo.lang.forEach(_5b1,function(node){
var top=null;
var left=null;
var init=(function(){
var _5bc=node;
return function(){
var pos=_5b7(_5bc,"position");
top=(pos=="absolute"?node.offsetTop:parseInt(_5b7(node,"top"))||0);
left=(pos=="absolute"?node.offsetLeft:parseInt(_5b7(node,"left"))||0);
if(!dojo.lang.inArray(["absolute","relative"],pos)){
var ret=dojo.html.abs(_5bc,true);
dojo.html.setStyleAttributes(_5bc,"position:absolute;top:"+ret.y+"px;left:"+ret.x+"px;");
top=ret.y;
left=ret.x;
}
};
})();
init();
var anim=dojo.lfx.propertyAnimation(node,{"top":{start:top,end:top+(_5b2.top||0)},"left":{start:left,end:left+(_5b2.left||0)}},_5b3,_5b4).connect("beforeBegin",init);
if(_5b5){
anim.connect("onEnd",function(){
_5b5(_5b1,anim);
});
}
_5b6.push(anim);
});
return dojo.lfx.combine(_5b6);
};
dojo.lfx.html.explode=function(_5c0,_5c1,_5c2,_5c3,_5c4){
var h=dojo.html;
_5c0=dojo.byId(_5c0);
_5c1=dojo.byId(_5c1);
var _5c6=h.toCoordinateObject(_5c0,true);
var _5c7=document.createElement("div");
h.copyStyle(_5c7,_5c1);
if(_5c1.explodeClassName){
_5c7.className=_5c1.explodeClassName;
}
with(_5c7.style){
position="absolute";
display="none";
var _5c8=h.getStyle(_5c0,"background-color");
backgroundColor=_5c8?_5c8.toLowerCase():"transparent";
backgroundColor=(backgroundColor=="transparent")?"rgb(221, 221, 221)":backgroundColor;
}
dojo.body().appendChild(_5c7);
with(_5c1.style){
visibility="hidden";
display="block";
}
var _5c9=h.toCoordinateObject(_5c1,true);
with(_5c1.style){
display="none";
visibility="visible";
}
var _5ca={opacity:{start:0.5,end:1}};
dojo.lang.forEach(["height","width","top","left"],function(type){
_5ca[type]={start:_5c6[type],end:_5c9[type]};
});
var anim=new dojo.lfx.propertyAnimation(_5c7,_5ca,_5c2,_5c3,{"beforeBegin":function(){
h.setDisplay(_5c7,"block");
},"onEnd":function(){
h.setDisplay(_5c1,"block");
_5c7.parentNode.removeChild(_5c7);
}});
if(_5c4){
anim.connect("onEnd",function(){
_5c4(_5c1,anim);
});
}
return anim;
};
dojo.lfx.html.implode=function(_5cd,end,_5cf,_5d0,_5d1){
var h=dojo.html;
_5cd=dojo.byId(_5cd);
end=dojo.byId(end);
var _5d3=dojo.html.toCoordinateObject(_5cd,true);
var _5d4=dojo.html.toCoordinateObject(end,true);
var _5d5=document.createElement("div");
dojo.html.copyStyle(_5d5,_5cd);
if(_5cd.explodeClassName){
_5d5.className=_5cd.explodeClassName;
}
dojo.html.setOpacity(_5d5,0.3);
with(_5d5.style){
position="absolute";
display="none";
backgroundColor=h.getStyle(_5cd,"background-color").toLowerCase();
}
dojo.body().appendChild(_5d5);
var _5d6={opacity:{start:1,end:0.5}};
dojo.lang.forEach(["height","width","top","left"],function(type){
_5d6[type]={start:_5d3[type],end:_5d4[type]};
});
var anim=new dojo.lfx.propertyAnimation(_5d5,_5d6,_5cf,_5d0,{"beforeBegin":function(){
dojo.html.hide(_5cd);
dojo.html.show(_5d5);
},"onEnd":function(){
_5d5.parentNode.removeChild(_5d5);
}});
if(_5d1){
anim.connect("onEnd",function(){
_5d1(_5cd,anim);
});
}
return anim;
};
dojo.lfx.html.highlight=function(_5d9,_5da,_5db,_5dc,_5dd){
_5d9=dojo.lfx.html._byId(_5d9);
var _5de=[];
dojo.lang.forEach(_5d9,function(node){
var _5e0=dojo.html.getBackgroundColor(node);
var bg=dojo.html.getStyle(node,"background-color").toLowerCase();
var _5e2=dojo.html.getStyle(node,"background-image");
var _5e3=(bg=="transparent"||bg=="rgba(0, 0, 0, 0)");
while(_5e0.length>3){
_5e0.pop();
}
var rgb=new dojo.gfx.color.Color(_5da);
var _5e5=new dojo.gfx.color.Color(_5e0);
var anim=dojo.lfx.propertyAnimation(node,{"background-color":{start:rgb,end:_5e5}},_5db,_5dc,{"beforeBegin":function(){
if(_5e2){
node.style.backgroundImage="none";
}
node.style.backgroundColor="rgb("+rgb.toRgb().join(",")+")";
},"onEnd":function(){
if(_5e2){
node.style.backgroundImage=_5e2;
}
if(_5e3){
node.style.backgroundColor="transparent";
}
if(_5dd){
_5dd(node,anim);
}
}});
_5de.push(anim);
});
return dojo.lfx.combine(_5de);
};
dojo.lfx.html.unhighlight=function(_5e7,_5e8,_5e9,_5ea,_5eb){
_5e7=dojo.lfx.html._byId(_5e7);
var _5ec=[];
dojo.lang.forEach(_5e7,function(node){
var _5ee=new dojo.gfx.color.Color(dojo.html.getBackgroundColor(node));
var rgb=new dojo.gfx.color.Color(_5e8);
var _5f0=dojo.html.getStyle(node,"background-image");
var anim=dojo.lfx.propertyAnimation(node,{"background-color":{start:_5ee,end:rgb}},_5e9,_5ea,{"beforeBegin":function(){
if(_5f0){
node.style.backgroundImage="none";
}
node.style.backgroundColor="rgb("+_5ee.toRgb().join(",")+")";
},"onEnd":function(){
if(_5eb){
_5eb(node,anim);
}
}});
_5ec.push(anim);
});
return dojo.lfx.combine(_5ec);
};
dojo.lang.mixin(dojo.lfx,dojo.lfx.html);
dojo.kwCompoundRequire({browser:["dojo.lfx.html"],dashboard:["dojo.lfx.html"]});
dojo.provide("dojo.lfx.*");

