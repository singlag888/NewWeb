

function stop_ad2(){
	if(window.ad2Timer){
		clearInterval(window.ad2Timer);
	}
}
function start_ad2(current){
	var current_pic = current;
	var all_pic = $('#pic_window a').length;
	var times = all_pic*2-1;
	var direction = 1;	//向上滑动
	$('#pic_window').css('height',all_pic*90);
//	$('.pic_dot a.active').removeClass('active');
//	$('.pic_dot a:eq('+(current_pic-1)+')').addClass('active');
	if(('#ad2').length>1){
		window.ad2Timer = setInterval(function(){
			var top_v = -(current_pic-1)*90;
			$('#pic_window').animate({top:top_v+'px'});
			$('.pic_dot a.active').removeClass('active');
			$('.pic_dot a:eq('+(current_pic-1)+')').addClass('active');
			current_pic += direction;
			if(current_pic == 1 || current_pic == all_pic){
				direction = -direction;
			}
			if(current_pic < 1)
			{
				current_pic=1
			}
			if(current_pic > all_pic){
				current_pic = all_pic;
			}
		},30000);
	}
}
//游戏的顶部图片轮播
$(document).ready(function(){
	if($('#ad2').length > 1){
		start_ad2(1);
	}
	var cha;
	$('.pic_dot a').mouseover(function(){
		stop_ad2();
		$('#pic_window').animate({top:(-$(this).index()*90)+'px'});
		cha=$(this).index();
		$('.pic_dot a.active').removeClass('active');
		$(this).addClass('active');
	});
	$('.pic_dot a').mouseout(function(){       
	   if(cha==$('#pic_window a').length-1)
	   {start_ad2(1)}
	   else
	   start_ad2(cha+1);
	})
});