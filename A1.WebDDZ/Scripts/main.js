// Generated by CoffeeScript 1.9.3
(function() {
  $(document).ready(function() {
    $('#form-pay').submit(function(e) {
      var userId, userIdAgain;
      userId = $('.user-id').val();
      userIdAgain = $('.user-id-again').val();
      if (userId === '' || userIdAgain === '') {
        alert('账号不能为空');
        return false;
      }
      if (userId !== userIdAgain) {
        alert('两次输入的账号不一致');
        return false;
      }
    });
    return $('.js-custom-select ul li a').on('click', function() {
      var $inputEle, $this, $thisCustomSelect;
      $this = $(this);
      $thisCustomSelect = $this.closest('.js-custom-select');
      $inputEle = $thisCustomSelect.find('input.val');
      if ($this.closest('li').hasClass('active')) {
        return false;
      }
      $thisCustomSelect.find('ul li').removeClass('active');
      $this.closest('li').addClass('active');
      return $inputEle.val($this.attr('data-val'));
    });
  });

}).call(this);
