#!/bin/bash

for size in 16 24 32 48 64 96 128 256; do
    convert -resize $sizex$size -background none icon.psd[0] $size.png
done

for size in 16 24 32 48; do
  convert -colors 256 +dither $size.png png8:$size-8.png
  convert -colors 16  +dither $size-8.png $size-4.png
done

convert 16.png 24.png 32.png 48.png 16-8.png 24-8.png 32-8.png 48-8.png 16-4.png 24-4.png 32-4.png 48-4.png 64.png 96.png 128.png 256.png icon.ico

rm 16.png 24.png 32.png 48.png 16-8.png 24-8.png 32-8.png 48-8.png 16-4.png 24-4.png 32-4.png 48-4.png 64.png 96.png 128.png 256.png 