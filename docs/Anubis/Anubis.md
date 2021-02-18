# Anubis
Anubis is a steganography library. Steganography is the art of hiding information in other media, for example pictures. Using Anubis, you can hide data in images and wave audio files. Anubis also allows you to retrive previously stored data in these forms of media.

Anubis uses the `Jector` to "inject", that is, embed data in the image, and "eject", that is, retrieve data from stored media.

## How it Works
Anubis stores data by modifying the LSB (Least Significant Bit) of each input byte to match the bit of the data being stored. For example, all bitmap images have pixels, which are point representations of specific color. Each pixel usually has three channels: Red, Green, Blue. PNG images also have the Alpha channel, which reflects to transparency. Each pixel channel is usually stored in a single byte, and the values of ARGB usually start at 0, to 255. Different colors are created by varying different values, for example, the color red has the following values: {R: 255, G: 0, B: 0}, while the color white has the values: {255, 255, 255}. The color purple looks something like {255, 0, 255}

With that in mind, we can look at how a byte usually looks like at the bit level. As you probably know, a `byte` is composed of 8 bits. Therefore, examine the byte provided below:

 1110 0111

This is the binary representation for the number 231. The Least Significant Bit is that bit, which if modified, has very little effect on the overall number itself. For example, modifying the last 1 to 0 (rightmost), to make 1110 0110 gives the number 230. 230 and 231 are not very far apart. The Most Significant bit however, is that bit, that if modified, has drastic effect on the number itself. For example, modifying the first 1 to 0 (leftmost) to give  0110 0111 gives the number 103. As you can see, 231 and 103 are far apart.

With this in mind, and returning to our pixel channels. Imagine changing the least significant bit for a specific channel. There should be very little difference between the overall output pixel and the original pixel. Consider the image below:

![](..\..\assets\images\Docs\Anubis\lsb-difference-demonstration.png)

As you can see, there is no perceptible difference between these two images, though they both have different values of RGB.