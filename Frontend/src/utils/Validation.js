import Common from './Common';

const Video = {
    name: (value) => value?.length >= 3 && value?.length <= 255,
    desc: (value) => !value?.length || value?.length >= 5 && value?.length <= 5000,
    url: (value) => /^(ftps?|https?|rtsp|rtmp|udp):\/\/\S+$/.test(value),
    maxDuration: (value) => Common.parseTime(value) >= 3 && Common.parseTime(value) <= 14400
}

const Valitation = {
    Video
}

export default Valitation;