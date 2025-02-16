#version 330 core
out vec4 FragColor;

in vec2 texCoord;
in vec3 Color;

uniform bool wireframeMode;
uniform sampler2D texture0;

void main() {
    if(!wireframeMode) {
        FragColor = texture(texture0, texCoord);
        FragColor *= vec4(Color, 1.0f);
    }
    else {
        FragColor = vec4(0.0f);
    }
}
