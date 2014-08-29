$(function () {
    var canvas = new fabric.Canvas('cvPreview');

    wrapCanvasText = function (text, fontSize, fontFamily, maxW, maxH) {

        if (typeof maxH === "undefined") { maxH = 0; }
        var words = text.split(" ");
        var formatted = '';

        // calc line height
        var lineHeight = new fabric.Text(text, {
            fontFamily: fontFamily,
            fontSize: fontSize
        }).height;

        // adjust for vertical offset
        var maxHAdjusted = maxH > 0 ? maxH - lineHeight : 0;
        var context = canvas.getContext("2d");

        context.font = fontSize + "px " + fontFamily;
        var currentLine = '';
        var breakLineCount = 0;

        n = 0;
        while (n < words.length) {
            var isNewLine = currentLine == "";
            var testOverlap = currentLine + ' ' + words[n];

            // are we over width?
            var w = context.measureText(testOverlap).width;

            if (w < maxW) {  // if not, keep adding words
                if (currentLine != '') {
                    currentLine += ' ';
                }
                currentLine += words[n];
            }
            else {
                // if this hits, we got a word that need to be hypenated
                if (isNewLine) {
                    var wordOverlap = "";

                    // test word length until its over maxW
                    for (var i = 0; i < words[n].length; ++i) {

                        wordOverlap += words[n].charAt(i);
                        var withHypeh = wordOverlap + "-";

                        if (context.measureText(withHypeh).width >= maxW) {
                            // add hyphen when splitting a word
                            withHypeh = wordOverlap.substr(0, wordOverlap.length - 2) + "-";
                            // update current word with remainder
                            words[n] = words[n].substr(wordOverlap.length - 1, words[n].length);
                            formatted += withHypeh; // add hypenated word
                            break;
                        }
                    }
                }

                formatted += currentLine + '\n';
                breakLineCount++;
                currentLine = "";

                continue; // restart cycle
            }
            if (maxHAdjusted > 0 && (breakLineCount * lineHeight) > maxHAdjusted) {
                // reduce font size if whole text does not fit in the specified window
                return wrapCanvasText(text, fontSize - 5, fontFamily, maxW, maxH);
            }
            n++;
        }

        if (currentLine != '') {
            formatted += currentLine + '\n';
            breakLineCount++;
            currentLine = "";
        }

        // get rid of empy newline at the end
        return [formatted.substr(0, formatted.length - 1), fontSize];
    }

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

                imgInstance.sendToBack();
                // Adjust canvas height
                canvas.setHeight(bottom + imgInstance.currentHeight);
            }
        });
    });
    $('.memeCaption').keypress(function (event) {
        var keyCode = (event.which ? event.which : event.keyCode);
        if (keyCode === 10 || keyCode == 13) {

            if ($.trim($(this).val()).length == 0) {
                return;
            }

            var initialFontSize = 80;
            var fontFamily = 'Impact';
            var maxWidth = canvas.width - 50;
            var maxHeight = canvas.height / 3;

            var wrappedTextData = wrapCanvasText(
                $(this).val(),
                initialFontSize,
                fontFamily,
                maxWidth,
                maxHeight
            );

            var text = new fabric.Text(wrappedTextData[0], {
                stroke: '#000',
                fill: '#fff',
                strokeWidth: 2,
                fontSize: wrappedTextData[1],
                fontWeight: 'bold',
                fontFamily: fontFamily
            });

            canvas.add(text);


            if ($(this).hasClass('topCaption')) {
                text.top = 0;
            }
            else if ($(this).hasClass('bottomCaption')) {
                text.top = canvas.height - text.currentHeight;
            }
            else if ($(this).hasClass('genericCaption')) {
                text.top = (canvas.height - text.currentHeight) / 2;
            }

            text.centerH();
            text.setCoords();
        }
    });
});