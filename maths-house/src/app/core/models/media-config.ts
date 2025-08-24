export interface MediaConfig {
  houseImage: string; // main house image
  handImage: string; // pointer/hand icon
  startupVideo: string; // intro video
  startupAudio: string; // intro audio
}

export const defaultMediaConfig: MediaConfig = {
  houseImage: 'assets/images/math_house.png',
  handImage: 'assets/images/access_granted.png',
  // Use files that exist under src/assets so they are bundled into /assets
  startupVideo: 'assets/video/ZStartingVideo.mp4',
  startupAudio: 'assets/audio/evilwitch.wav',
};
