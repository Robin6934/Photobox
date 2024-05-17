package com.PhotoboxWeb.DTOs;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.ToString;

@Getter
@Setter
@NoArgsConstructor
@ToString
public class PolingDTO {

    private boolean triggerPicture;
    private String printPictureName;
}
