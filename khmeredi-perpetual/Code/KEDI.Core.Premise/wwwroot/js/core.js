class _$_{static isJSON(value){return value!==undefined&&value.constructor===Object;}
static isValidJSON(json){return json!==undefined&&json.constructor===Object&&Object.keys(json).length>0;}
static isArray(value){return value!==undefined&&Array.isArray(value);}
static isValidArray(value){return value!==undefined&&Array.isArray(value)&&value.length>0;}
static isHTML(value){return value!==undefined&&(/<[a-z/][\s\S]*>/i).test(value);}
static validNumber(value){if(value=="00"){value="0";}
if(value==="."){value="0.";}
if(!value.includes(".")){value=parseFloat(value);}
if(!(/^[-]?\d+[.]?\d*$/).test(value)){value=value.toString().substring(0,value.length-1);}
return value;}
static isUniform(a,b,equal){let identical=false;if(_$_.isValidJSON(a)&&_$_.isValidJSON(b)){$.each(a,function(ak,av){$.each(b,function(bk,bv){identical=ak===bk;});});}
if(a!==undefined&&!_$_.isValidJSON(a)){if(b!==undefined&&!_$_.isValidJSON(b)){return a===b;}}
return identical;}
static targetBound(target,event_type){let bound=false
if(target!==undefined){$.each($._data(target,"events"),function(k,v){if(k===event_type){bound=true;}});}
return bound;}
static highestZindex(highest,tag_name){let tag="*";if(tag_name!==undefined){tag=tag_name;}
$(tag).each(function(){let zindex=$(this).css("z-index");if((zindex>highest)&&(zindex!='auto')){highest=zindex;}});return highest;}
static isEmpty(value){return value===undefined||value===""||value===null;}}$.fn.dragScroll=function(options={}){var cur_down=false;var cur_x_pos=0;var cur_y_pos=0;var container=$(this);container.addClass("ds-grab");if(typeof container==="undefined"){return;}
$(window).mousemove(function(m){if(cur_down===true){var cal_pos_x=(cur_x_pos-m.pageX);var cal_pos_y=(cur_y_pos-m.pageY);container.scrollLeft(cal_pos_x);container.scrollTop(cal_pos_y);}});container.mousedown(function(m){container.removeClass("ds-grab");container.addClass("ds-grabbing");cur_down=true;cur_x_pos=m.pageX+$(this).scrollLeft();cur_y_pos=m.pageY+$(this).scrollTop();m.preventDefault();});$(window).mouseup(function(){cur_down=false;container.removeClass("ds-grabbing");container.addClass("ds-grab");});};class DialogBuilder{constructor(config){this._background=$("<div class='dialog-box-background'></div>");this._self=$("<div class='dialog-box'></div>");this._header=$("<div class='header'></div>");this._content=$("<div class='content'></div>");this._footer=$("<div class='footer'></div>");this._setting={self:this.self,class:"",container:{selector:"",attribute:"dialog-container"},insertion:"append",caption:"",content:"",content_type:"text",type:"",icon:"",position:"",button:{meta:this},animation:{startup:{complete:false,timing:"after",name:"slide-down",delay:100,duration:300,callback:function(e){}},shutdown:{complete:false,timing:"after",name:"fade",delay:0,duration:300,callback:function(e){},keycode:27,}}}
this.setting=config;}
set setting(config){if(!!config&&config.constructor===Object&&Object.keys(config).length>0){this._setting=$.extend(true,{},this._setting,config);}}
get setting(){return this._setting;}
get background(){return this._background;}
get self(){return this._self;}
get header(){return this._header;}
get content(){return this._content;}
get footer(){return this._footer;}
addOrigin(process,name){this.setting.animation[process].name=name;switch(name){case"slide-up":if(process==="startup"){this.self.addClass("origin-bottom");}
if(process==="shutdown"){this.self.addClass("origin-top");}
break;case"slide-down":if(process==="startup"){this.self.addClass("origin-top");}
if(process==="shutdown"){this.self.addClass("origin-bottom");}
break;case"slide-left":if(process==="startup"){this.self.addClass("origin-right");}
if(process==="shutdown"){this.self.addClass("origin-left");}
break;case"slide-right":if(process==="startup"){this.self.addClass("origin-left");}
if(process==="shutdown"){this.self.addClass("origin-right");}
break;case"fade":this.self.addClass("fade");break;}}
removeOrigin(process,name){this.setting.animation[process].name=name;switch(name){case"slide-up":this.self.removeClass("origin-bottom");break;case"slide-down":this.self.removeClass("origin-top");break;case"slide-left":this.self.removeClass("origin-right");break;case"slide-right":this.self.removeClass("origin-left");break;case"fade":this.self.removeClass("fade");break;}}
animate(process,delay,duration,name){this.setting.animation[process].delay=delay;this.setting.animation[process].duration=duration;this.setting.animation[process].name=name;let timespan=parseFloat(duration)/1000;setTimeout(()=>{switch(process){case"startup":Promise.all([this.background.addClass("blur").css("transition",timespan+"s"),this.self.addClass("startup").css("transition",timespan+"s")]);break;case"shutdown":Promise.all([this.self.removeClass("startup").addClass("shutdown").css("transition",timespan+"s"),this.background.removeClass("blur").css("transition",timespan+"s")]);break;}},parseFloat(this.setting.animation[process].delay));}
onEffect(process,timing,callback){this.setting.animation[process].timing=timing;this.setting.animation[process].callback=callback;}
startup(timing,callback){if(!!timing&&typeof timing==="string"){this.setting.animation.startup.timing=timing;this.setting.animation.startup.callback=callback;}
let _startup=this.setting.animation.startup;switch(_startup.timing.toLowerCase()){case"before":Promise.resolve(setTimeout(()=>{_startup.callback(this,this.self);})).then(Promise.all([this.build(this.setting),this.setPosition(this.setting.position),this.addOrigin("startup",this.setting.animation.startup.name),this.animate("startup",_startup.delay,_startup.duration,_startup.name)]));break;case"during":Promise.resolve(this.build(this.setting),this.setPosition(this.setting.position),this.addOrigin("startup",this.setting.animation.startup.name),this.animate("startup",_startup.delay,_startup.duration,_startup.name),setTimeout(()=>{_startup.callback(this,this.self)}));break;case"after":Promise.resolve(this.build(this.setting),this.setPosition(this.setting.position),this.addOrigin("startup",this.setting.animation.startup.name),this.animate("startup",_startup.delay,_startup.duration,_startup.name)).then(setTimeout(()=>{_startup.callback(this,this.self);},parseFloat(_startup.delay)+parseFloat(_startup.duration)));break;}
this.onEffect("startup",_startup.timing,_startup.callback);this.setting.animation.startup.complete=true;}
shutdown(timing,callback){if(this.setting.animation.startup.complete){if(!!timing&&typeof timing==="string"){this.setting.animation.shutdown.timing=timing;this.setting.animation.shutdown.callback=callback;}
let _shutdown=this.setting.animation.shutdown;switch(_shutdown.timing.toLowerCase()){case"before":_shutdown.callback(this,this.self);Promise.resolve(this.removeOrigin("startup",this.setting.animation.startup.name),this.addOrigin("shutdown",this.setting.animation.shutdown.name),this.animate("shutdown",_shutdown.delay,_shutdown.duration,_shutdown.name));break;case"during":Promise.all([this.removeOrigin("startup",this.setting.animation.startup.name),this.addOrigin("shutdown",this.setting.animation.shutdown.name),this.animate("shutdown",_shutdown.delay,_shutdown.duration,_shutdown.name),_shutdown.callback(this,this.self)]);break;case"after":Promise.resolve(this.removeOrigin("startup",this.setting.animation.startup.name),this.addOrigin("shutdown",this.setting.animation.shutdown.name),this.animate("shutdown",_shutdown.delay,_shutdown.duration,_shutdown.name));setTimeout(()=>{_shutdown.callback(this,this.self);},parseFloat(_shutdown.delay)+parseFloat(_shutdown.duration));break;}
setTimeout(()=>{this.onEffect("shutdown",_shutdown.timing,_shutdown.callback);this.destroy();},parseFloat(this.setting.animation.shutdown.delay)
+parseFloat(this.setting.animation.shutdown.duration));this.setting.animation.shutdown.complete=true;}}
setPosition(value){if(value!==undefined){this.setting.position=value;switch(this.setting.position){case"top-left":this.background.addClass("top-left");break;case"top-center":this.background.addClass("top-center");break;case"top-right":this.background.addClass("top-right");break;case"center-left":this.background.addClass("center-left");break;case"center":this.background.addClass("center");break;case"center-right":this.background.addClass("center-right");break;case"bottom-left":this.background.addClass("bottom-left");break;case"bottom-center":this.background.addClass("bottom-center");break;case"bottom-right":this.background.addClass("bottom-right");break;}}}
build(config){}
async destroy(){}
highestZIndex(tag_name){let tag="*";let highest=0;if(!!tag_name){tag=tag_name;}
$(tag).each(function(){let zindex=$(this).css("z-index");if((zindex>highest)&&(zindex!='auto')){highest=zindex;}});return highest;}}
class DialogBox extends DialogBuilder{constructor(config){super(config);this.setting={type:"ok",icon:"info",close_button:true,position:"top-center",button:{yes:{meta:this,text:"Yes",callback:function(e){},keycode:13},no:{meta:this,text:"No",callback:function(e){},keycode:undefined},ok:{meta:this,text:"Ok",callback:function(e){},keycode:13},cancel:{meta:this,text:"Cancel",callback:function(e){},keycode:27}}}
this.setting=config;this.startup();}
confirm(callback){this.setting.button.ok.callback=callback;this.setting.button.yes.callback=callback;}
reject(callback){this.setting.button.no.callback=callback;this.setting.button.cancel.callback=callback;}
get caption(){let $caption=$("<div class='caption'></div>");if(this.setting.caption!==undefined){$caption.append(this.setting.caption);}
return $caption;}
get closeButton(){let $this=this;let $wrap_icon=$("<div class='wrap-icon'></div>");let $close_button=$();if(this.setting.close_button===true){$close_button=$("<i class='icon icon-close'></i>");}else{if(typeof this.setting.close_button==="string"){$close_button=$("<i class='"+this.setting.close_button+"'></i>");}}
return $wrap_icon.append($close_button.on("click",function(e){$this.shutdown();}));}
get icon(){let $wrap_icon=$("<div class='wrap-icon'></div>");let $icon=$("<div class='icon'></div>");if(this.setting.icon!==undefined){switch(this.setting.icon){case"info":$icon.addClass("info");break;case"warning":$icon.addClass("warning");break;case"danger":$icon.addClass("danger");break;default:if(!!this.setting.icon&&this.setting.icon.constructor===Object&&Object.keys(this.setting.icon).length>0){let icon=this.setting.icon;let $i_icon=$("<i></i>");if(icon.class!==undefined){$i_icon.addClass(icon.class);}
if(icon.color!==undefined){$i_icon.css("color",icon.color);}
if(icon.background_color!==undefined){$icon.css("background",icon.background_color);}
$icon.append($i_icon);}
if(typeof this.setting.icon==="string"){$icon.append("<i class='"+this.setting.icon+"'></i>");if(!!this.setting.icon&&(/<[a-z/][\s\S]*>/i).test(this.setting.icon)){$icon.append(this.setting.icon);}}
if(this.setting.icon==="none"||this.setting.icon==null){$icon=$();}}}
return $wrap_icon.append($icon);}
get button(){let $button_wrap=$("<div class='wrap-button'></div>");if(this.setting.button==="none"){$button_wrap.addClass("free-space");}
if(!!this.setting.button&&this.setting.button.constructor===Object&&Object.keys(this.setting.button).length>0){let type=this.setting.type.toLowerCase();let button=this.setting.button;let $ok=$("<div class='button'>"+button.ok.text+"</div>");let $yes=$("<div class='button'>"+button.yes.text+"</div>");let $no=$("<div class='button'>"+button.no.text+"</div>");let $cancel=$("<div class='button'>"+button.cancel.text+"</div>");switch(type){case"ok":if(typeof button.ok.callback==="function"){$ok.on("click",function(e){button.ok.callback(e);if(typeof button.meta.setting.content==="string"){button.meta.shutdown();}});}
$button_wrap.append($ok).addClass("right");break;case"yes-no":case"yes/no":if(typeof button.yes.callback==="function"){$yes.on("click",function(e){button.yes.callback(e);});}
if(typeof button.no.callback==="function"){$no.on("click",function(e){button.no.callback(e);if(typeof button.meta.setting.content==="string"){button.meta.shutdown();}});}
$button_wrap.append($yes).append($no).addClass("right");break;case"ok-cancel":case"ok/cancel":if(typeof button.ok.callback==="function"){$ok.on("click",function(e){button.ok.callback(e);});}
if(typeof button.cancel.callback==="function"){$cancel.on("click",function(e){button.cancel.callback(e);if(typeof button.meta.setting.content==="string"){button.meta.shutdown();}});}
$button_wrap.append($ok).append($cancel).addClass("right");break;case"yes-no-cancel":case"yes/no/cancel":if(typeof button.yes.callback==="function"){$yes.on("click",function(e){button.yes.callback(e);});}
if(typeof button.no.callback==="function"){$no.on("click",function(e){button.no.callback(e);});}
if(typeof button.cancel.callback==="function"){$cancel.on("click",function(e){button.cancel.callback(e);if(typeof button.meta.setting.content==="string"){button.meta.shutdown();}});}
$button_wrap.append($yes).append($no).append($cancel).addClass("right");break;default:if(this.setting.type==="none"){return $button_wrap.addClass("free-space");}}}
return $button_wrap;}
build(config){this.destroy();this.setting=config;this.header.children().remove();this.header.append(this.icon).append(this.caption).append(this.closeButton);if(this.setting.content!==undefined){this.content.html("<span>"+this.setting.content+"</span>");if(!!this.setting.content&&this.setting.content.constructor===Object&&Object.keys(this.setting.content).length>0){if(this.setting.content.selector.includes("#")||this.setting.content.selector.includes(".")){let $cloned=$(this.setting.content.selector).clone(true);this.content.html($cloned).addClass("outer-content");if(this.setting.content.class!==undefined){this.content.removeClass("outer-content").addClass(this.setting.content.class);}}}}
this.footer.html(this.button);this.self.append(this.header).append(this.content).append(this.footer);this.background.append(this.self);let container=this.setting.container.selector;if(!!this.setting.container.attribute){container=$("["+this.setting.container.attribute+"]");}
if(!!container){let $this=this,insertion=this.setting.insertion;document.addEventListener("keyup",function(e){let _keycode=$this.setting.animation.shutdown.keycode;if(typeof $this.setting.animation.shutdown.keycode==="string"){_keycode=$this.setting.animation.shutdown.keycode.toLowerCase();}
if(e.which===_keycode){$this.shutdown();}});switch(insertion.toLowerCase()){case"replace":container.html(this.background);break;case"prepend":container.prepend(this.background);break;case"append":container.append(this.background);break;}
container.css("position","fixed").css("z-index","2147483647");}}
destroy(){let container=this.setting.container.selector;if(!!this.setting.container.attribute){container=$("["+this.setting.container.attribute+"]");}
if(this instanceof DialogBox){delete this;this.background.remove();container.children().remove();}}}
class DialogCalculator extends DialogBuilder{constructor(config){super(config);this._left=0;this._right=0;this._result=0;this.setting={position:"top-center",output:this.result,button:{accept:{meta:this,text:"Accept",callback:function(e){}}},animation:{shutdown:{name:"slide-up"}}}
Number.prototype.precise=function(max_precision){return(this.toString().length>max_precision)?this.toPrecision(max_precision):this;}
Math.negative=function(value){return parseFloat(value)<0;}
Math.negate=function(value){return parseFloat(value)*(-1);}
this.setting=config;this.refresh();}
set result(value){this._result=value;}
get result(){return this._result;}
set left(value){this._left=parseFloat(value);}
get left(){return this._left;}
set right(value){this._right=parseFloat(value);}
get right(){return this._right;}
multiply(){this._result=this._left*this._right;return this._result;}
divide(){if(this._right!==0){this._result=this._left / this._right;}else{if(Math.negative(this._left)){this._result=Number.NEGATIVE_INFINITY;}else{this._result=Number.POSITIVE_INFINITY;}}
return this._result;}
add(){this._result=this._left+this._right;return this._result;}
substract(){this._result=this._left-this._right;return this._result;}
accept(callback){this.setting.button.accept.callback=callback;}
async build(setting){let $this=this;this.setting=setting;this.self.addClass("calculator standard");let $content=$("<div class='navigator reserve'></div>"
+"<div class='navigator output'>0</div>"
+"<div class='keypad'>"
+"<div data-key='ce' class='key control'>CE</div>"
+"<div data-key='c' class='key control'>C</div>"
+"<div data-key='backspace' class='key control'><i class='backspace'>×</i></div>"
+"<div data-key='÷' class='key operator'>÷</div>"
+"<div data-key='7' class='key operant'>7</div>"
+"<div data-key='8' class='key operant'>8</div>"
+"<div data-key='9' class='key operant'>9</div>"
+"<div data-key='×' class='key operator'>×</div>"
+"<div data-key='4' class='key operant'>4</div>"
+"<div data-key='5' class='key operant'>5</div>"
+"<div data-key='6' class='key operant'>6</div>"
+"<div data-key='-' class='key operator'>-</div>"
+"<div data-key='1' class='key operant'>1</div>"
+"<div data-key='2' class='key operant'>2</div>"
+"<div data-key='3' class='key operant'>3</div>"
+"<div data-key='+' class='key operator'>+</div>"
+"<div data-key='±' class='key control'>±</div>"
+"<div data-key='0' class='key operant'>0</div>"
+"<div data-key='.' class='key operant'>.</div>"
+"<div data-key='=' class='key r-operator'>=</div>"
+"</div>");this.self.html($content).append($("<div class='navigator accept-result'>Accept</div>").on("click",function(e){setting.button.accept.callback(e);}));this.calculate(this.self);this.background.html(this.self).on("click",function(e){if($(e.target).is(this)){$this.shutdown();}});$(this.setting.container).append(this.background);};calculate($calc){const $operant=$calc.find('.keypad .key.operant');const $operator=$operant.siblings(".operator");const $r_operator=$operant.siblings(".r-operator");const $output=$operant.parent().siblings(".output");const $reserve=$output.siblings(".reserve");const $control=$operant.siblings(".control");let output=0;let reserve="";let sign="";let ready=false;$operant.click(function(e){if(ready){output=$(this).data('key').toString();}else{if($output.text().length<13){output+=$(this).data('key').toString();}}
output=_$_.validNumber(output);$output.text(output);ready=false;});$operator.click(function(e){resultClicked(e);sign=$(this).data("key").toString();calc.left=parseFloat(output);reserve=calc.left.precise(9)+" "+sign;$(".reserve").text(reserve);$(".output").text(output);ready=true;});$r_operator.click(resultClicked);function resultClicked(e){if(sign===""){return;}
calc.right=parseFloat(output);reserve=calc.left.precise(6)+" "+sign+" "+calc.right.precise(6);if(Math.negative(calc.right)){reserve=calc.left.precise(6)+" "+sign+" ( "+calc.right.precise(6)+" )";}
output="";switch(sign){case"×":calc.multiply();break;case"÷":calc.divide();break;case"+":calc.add();break;case"-":calc.substract();break;}
output=calc.result.precise(9);$reserve.text(reserve);$output.text(output);ready=true;}
$control.click(function(e){output=output.toString();switch($(this).data("key")){case'±':output=Math.negate(output).toString();break;case'backspace':if(output.includes("Infinity")){output=0;}else{output=output.substring(0,output.length-1);if(output.length<2&&output.includes("-")){output=0;}}
break;case'c':calc.left=0;calc.right=0;calc.result=0;output=0;sign="";reserve="";break;case'ce':output=0;break;}
if(output.length===0){output=0;}
$reserve.text(reserve);$output.text(output);});}}
$(function(){let wrap_input=$(".group-search-boxes .wrap-search-box .wrap-input");wrap_input.siblings(".btn-search").click(function(e){$(this).siblings(".wrap-input").addClass("show");$(this).parent().parent().find(".btn-search").hide();setTimeout(()=>{wrap_input.find("input").focus().val("");},250);if($(this).parent().parent().parent().hasClass("nav-toolbar")){$(this).parent().parent().siblings().addClass("hide");}});wrap_input.find("#i-close").click(function(e){e.preventDefault();$(this).parent().removeClass("show");$(this).parent().parent().parent().find(".btn-search").show();$(this).parent().parent().parent().siblings().removeClass("hide");});});class Dropbox{static get target(){return"html, .dropbox-btn, .i-close";}
constructor(container,config=0){this._target="html, #demo-order-list .dropbox-btn, #demo-order-list .i-close";this._self=$();this._setting={container:container,direction:"down",insertion:"replace",on_target:"mousedown",type:"list",has_crumb:true,trigger:{icon:"fas fa-list-alt"},content:{add_option:{class:"fas fa-plus",listener:[]},scroll:"vertical",data:[],option:{has_title:false,icon:"fas fa-receipt",listener:[]}}}
this.setting=config;this.settingUpdated();Array.prototype.where=function(condition){return $.grep(this,function(json,i){return condition(json,i);});};Array.prototype.first=function(condition){return $.grep(this,function(json,i){return condition(json,i);})[0];};Array.prototype.remove=function(key){if(_$_.validArray(key)){for(let k of key){this.remove(k);}}else{let item={};item=this.first(function(item){return item.key===key;});if(this.indexOf(item)!==-1){this.splice(this.indexOf(item),1);}}}}
get target(){return this._target;}
get self(){return this._self;}
get setting(){return this._setting;}
set setting(config){if(_$_.isValidJSON(config)){this._setting=$.extend(true,{},this._setting,config);}}
async settingUpdated(){await Promise.resolve(setTimeout(()=>{this.refresh();},0));}
refresh(){this.init(this.setting);}
moveScrollY(selector,position){switch(position){case"top":$(selector).scrollTop(0);break;case"bottom":$(selector).scrollTop($(selector)[0].scrollHeight);break;default:$(selector).scrollTop($(selector)[0].scrollHeight);}}
onTarget(event_type){let target=this.target;let $self=this.self;$(target).on(event_type,function(e){e.stopPropagation();if($(this).is($self.find(".dropbox-btn"))){$(this).toggleClass("show");$(this).siblings(".wrap-dropbox-content").toggleClass("show");}
if($(this).is($self.find(".i-close"))){$self.find(".dropbox-btn").removeClass("show");$self.find(".wrap-dropbox-content").removeClass("show");}
if($(this).is($self.parents())&&!$(e.target).is($self.find("*"))){$self.find(".dropbox-btn").removeClass("show");$self.find(".wrap-dropbox-content").removeClass("show");}});}
static async onTarget(event_type){let selector=Dropbox.target;$(selector).on(event_type,function(e){e.stopPropagation();if($(this).is($(".dropbox-btn"))){$(this).toggleClass("show");$(this).siblings(".wrap-dropbox-content").toggleClass("show");}
if($(this).is($(".i-close"))){$(this).parent().parent().siblings(".dropbox-btn").removeClass("show");$(this).parent().parent(".wrap-dropbox-content").removeClass("show");}
if($(this).is($("html"))&&!$(e.target).is($(".dropbox *"))){$(".dropbox .dropbox-btn").removeClass("show");$(".dropbox .wrap-dropbox-content").removeClass("show");}});}
async buildTrigger($dropbox){let $dropbox_btn=$("<div class='dropbox-btn'></div>");let $badge=$("<div class='badge'></div>");let $icon=$("<i class='icon'></i>");let $img_circle=$("<img class='img-circle' src='/images/user.png'/>");if($dropbox.hasClass("list")){if(_$_.isValidJSON(this.setting)){if(_$_.isHTML(this.setting.trigger.icon)){$icon.replaceWith(this.setting.trigger.icon);}else{if(typeof this.setting.trigger.icon==="string"){$icon.addClass(this.setting.trigger.icon);}}
if(this.setting.trigger.badge!==undefined){$badge.append(this.setting.trigger.badge);}}
$dropbox_btn.append($icon).append($badge);}
if($dropbox.hasClass("profile")){if(_$_.isValidJSON(this.setting)){if(_$_.isHTML(this.setting.trigger.icon)){$img_circle.replaceWith(this.setting.trigger.icon);}else{$img_circle.prop("src",this.setting.trigger.icon);}}
$dropbox_btn.append($img_circle);}
$dropbox.append($dropbox_btn);}
async buildContent($dropbox){let $wrap_dropbox_content=$("<div class='wrap-dropbox-content'></div>");$("<div class='dropbox-navigator'>"
+"<div class='i-close far fa-window-close'></div>"
+"</div>").appendTo($wrap_dropbox_content);let $dropbox_content=$("<div class='dropbox-content'></div>");let add_opt=this.setting.content.add_option;if($dropbox.hasClass("list")){if(_$_.isValidJSON(add_opt)){let $add_option=$("<i class='add-option'></i>");if(add_opt.class!==undefined){$add_option.addClass(add_opt.class);}
if(_$_.validArray(add_opt.listener)){$add_option.on(add_opt.listener[0],add_opt.listener[1]);}
if(_$_.isValidJSON(add_opt.listener)){$add_option.on(add_opt.listener.event,add_opt.listener.callback);}
$dropbox_content.append($add_option);}}
let $wrap_option=$("<div class='wrap-option'></div>");switch(this.setting.content.scroll){case"vertical":$wrap_option.addClass("scroll-y");break;case"none":$wrap_option.removeClass("scroll-y");break;default:$wrap_option.addClass("scroll-y");}
let data=this.setting.content.data;let opt=this.setting.content.option;if(_$_.validArray(data)){data.forEach(function(d){let $option=$("<a class='option' href='#' data-key='"+d.key+"'>"+" "+d.value+"</a>");let $icon=$("<i class='"+opt.icon+"'></i>");if(d.icon!==undefined){if(_$_.isHTML(d.icon)){$icon=d.icon;}else{$icon.addClass(d.icon);}}
$option.prepend($icon);if(d.url!==undefined){$option.prop("href",d.url);}
if(opt.has_title){$option.prop("title",d.value);}
if(d.title!==undefined){$option.prop("title",d.title);}
if(_$_.validArray(opt.listener)){$option.on(opt.listener[0],opt.listener[1]);}
if(_$_.isValidJSON(opt.listener)){$option.on(opt.listener.event,opt.listener.callback);}
$wrap_option.append($option);});}
$dropbox_content.append($wrap_option);$wrap_dropbox_content.append($dropbox_content);$dropbox.append($wrap_dropbox_content);}
async render(){let $dropbox=$("<div class='dropbox crumb'></div>");if(_$_.isValidJSON(this.setting)){if(this.setting.type!==undefined){$dropbox.addClass(this.setting.type);}
if(this.setting.direction!==undefined){$dropbox.addClass(this.setting.direction);}
if(this.setting.has_crumb!==undefined&&!this.setting.has_crumb){$dropbox.removeClass("crumb");}}
await this.buildTrigger($dropbox);await this.buildContent($dropbox);this._self=$dropbox;}
async init(config){if(!_$_.isValidJSON(config)){return;};this.setting=config;if(this.setting.container!==undefined){await this.render();switch(this.setting.insertion){case"prepend":$(this.setting.container).prepend(this.self);break;case"append":$(this.setting.container).append(this.self);break;case"replace":$(this.setting.container).html(this.self);break;default:$(this.setting.container).prepend(this.self);}
this.onTarget(this.setting.on_target);}}};$(function(){$("html, .dropbox-btn, .i-close").on("mousedown",function(e){if($(this).is($(".dropbox-btn"))){$(this).toggleClass("show");$(this).siblings(".wrap-dropbox-content").toggleClass("show");}
if($(this).is($(".i-close"))){$(this).parent().parent().siblings(".dropbox-btn").removeClass("show");$(this).parent().parent(".wrap-dropbox-content").removeClass("show");}
if($(this).is($("html"))&&!$(e.target).is($(".dropbox *"))){$(".dropbox .dropbox-btn").removeClass("show");$(".dropbox .wrap-dropbox-content").removeClass("show");}});});class AjaxLoader{constructor(image,config){this._self=$("<div class='ajax-loader'></div>");this._content=$("<div class='content'></div>");this._setting={container:"body",image:image}
if(config!==undefined){this.setting=config;}
this.refresh();}
set setting(config){if(_$_.isValidJSON(config)){this._setting=$.extend(true,{},this._setting,config);}}
get setting(){return this._setting;}
get self(){return this._self;}
get content(){return this._content;}
async refresh(){setTimeout(()=>{this.build(this.setting);},0);}
async build(config){this.setting=config;let $image=$("<img src='"+this.setting.image+"'/>");this.content.html($image);this.self.append(this.content);$(this.setting.container).append(this.self);}
async show(href){if(!!href){Promise.all([this.self.addClass("show"),location.href=href]);}else{this.self.addClass("show");}}
async hide(){this.self.removeClass("show");}
async close(){if(this instanceof AjaxLoader){setTimeout(()=>{delete this;this.self.remove();},0);}}};