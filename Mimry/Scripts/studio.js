$(function () {
    var canvas = new fabric.Canvas('cvPreview');

    $('#memeUrl').on('input', function (e) {
        $("<img>", {
            src: $(this).val(),
            load: function () {
                var scale = canvas.width / this.width;
                var imgInstance = new fabric.Image(this);
                imgInstance.scale(scale);
                imgInstance.selectable = false;

                var controls = canvas.getObjects();
                var bottom = 0;
                for (var i = 0; i < controls.length; i++) {
                    if (controls[i] instanceof fabric.Image) {
                        if (bottom < controls[i].top + controls[i].currentHeight) {
                            bottom = controls[i].top + controls[i].currentHeight;
                        }
                    }
                }
                imgInstance.top = bottom;
                
                canvas.add(imgInstance);

                // Adjust canvas height
                canvas.setHeight(bottom + imgInstance.currentHeight);
            }
        });
    });
});