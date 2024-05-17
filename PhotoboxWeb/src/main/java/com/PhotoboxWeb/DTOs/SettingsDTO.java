package com.PhotoboxWeb.DTOs;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.databind.JsonNode;
import lombok.*;

@Data
@JsonIgnoreProperties(ignoreUnknown = true)
public class SettingsDTO {

    private int CountDown;

    private String TextOnPicture;

    private String TextOnPictureFont;

    private int TextOnPictureFontSize;

    private String TextOnPictureColor;

    private int TextPositionFromRight;

    private int TextPositionFromBottom;

}
