Rick Sen 3472361
Nikè Lambooy 4090349

We split the work as follows:
- Rick did assignments E1, E6, M3
- Nikè did assignments E3, E5, M7. Unfortunately, Nikè ran out of time, so M7 is not finished/included.

References:
E3: 
	https://web.archive.org/web/20070106043736/http://www.shadertech.com/shaders/Checker.fx
E6:
	http://www.dhpoware.com/demos/xnaGaussianBlur.html#_blank (from assignment)
	http://gamedev.stackexchange.com/questions/18388/how-do-i-use-render-targets-in-xna
	http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series3/Render_to_texture.php
	https://msdn.microsoft.com/en-us/library/ff434402.aspx
M3:
	http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series4/Reflection_map.php (from assignment)
	http://iloveshaders.blogspot.com/2011/05/using-stencil-buffer-rendering-stencil.html (from assignment)
	

We did the following assignments
E1: The bunny model is displayed with 5 light sources. The number of light sources is easily adjusted, as required in the assignment.
E3: The head model is displayed with cell shading. Unfortunately, we were unable to implement anti-aliasing. See the effect file EffectE3 for more information.
E5: A grayscaled image is displayed. The grayscale is determined by the given formula. The grayscaling can be easily applied to a scene, when rendered to a RenderTarget before grayscaling, as in assignment E6.
E6: The bunny model is rendered to a texture, which is blurred using a gaussian distribution. The distribution size and the distribution itself can be easily set on load time.
M3: A mirror is drawn and the bunny model is reflected in it. This is achieved using stenciling. The mirrored scene is not rendered to a texture, but calculated on the fly, as in the assignment.
All assignments can be viewed with a demo camera using the space bar.