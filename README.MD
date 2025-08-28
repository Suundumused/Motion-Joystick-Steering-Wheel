# Motion Joystick

![Main Image](https://play-lh.googleusercontent.com/X0jYo-59ZZZK8wlTAXpeG3LOv5wzOop62n5156ZZKQvupwSYHM1L8U2FaCeWilh_ew=w240-h480-rw)

**Motion Joystick** is an open-source analog-based joystick project that uses the smartphone as a steering wheel or aircraft yoke by utilizing the accelerometer as the left analog stick. It contains 12 joystick model templates, and the triggers are analog with combined left and right trigger options.

[![Google Play Store Badge](https://raw.githubusercontent.com/Suundumused/Motion-Joystick-Steering-Wheel/refs/heads/main/Assets/Google_Play_Store_badge_EN.png)](https://play.google.com/store/apps/details?id=com.caiosilva.gravityjoystick&pcampaignid=web_share)

---

<div align="center">
    <a href="https://youtube.com/shorts/NQMqyPQ18os?si=p46ujCLKhOxtPgvC" target="_blank">
        <img src="https://raw.githubusercontent.com/Suundumused/Motion-Joystick-Steering-Wheel/refs/heads/main/Assets/Uxntitled.png" width="600" alt="Web Widget Preview"/>
    </a>
</div>

## Connection and Communication

The connection is made only via Wi-Fi, using the UDP protocol (with packet loss management) for low latency. The receiver on Windows creates a Virtual Joystick with the Xbox 360 controller profile and receives packets from the smartphone connected to the same network.


<div align="center">
    <img src="https://raw.githubusercontent.com/Suundumused/Motion-Joystick-Steering-Wheel/refs/heads/main/Assets/2232.png" width="600" alt="Web Widget Preview"/>
</div>

## Recommendations:
- It is recommended to attach the smartphone to a mobile phone holder with a flexible arm on the table for greater stability. Most models are designed to take up as much screen space as possible.
- Accelerometer data passes through stabilization and smoothing filters before being delivered to the final axis input.

<div align="center">
    <img src="https://raw.githubusercontent.com/Suundumused/Motion-Joystick-Steering-Wheel/refs/heads/main/Assets/151959.png" width="600" alt="Web Widget Preview"/>
</div>

## Project Areas

### Client Side
Access the client-side code [here](https://github.com/Suundumused/Motion-Joystick-Steering-Wheel/tree/main/Client%20Side).

### Server Side
Access the server-side code [here](https://github.com/Suundumused/Motion-Joystick-Steering-Wheel/tree/main/Server%20Side).
