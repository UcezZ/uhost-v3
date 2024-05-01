import Common from './Common';

const Auth = {
    login: (value) => value?.length >= 3 && value?.length <= 64,
    password: (value) => value?.length >= 3 && value?.length <= 64
}

const Video = {
    name: (value) => value?.length >= 3 && value?.length <= 255,
    desc: (value) => !value?.length || value?.length >= 5 && value?.length <= 5000,
    url: (value) => /^(ftps?|https?|rtsp|rtmp|udp):\/\/\S+$/.test(value),
    maxDuration: (value) => Common.parseTime(value) >= 3 && Common.parseTime(value) <= 14400
}

const User = {
    email: (value) => /^[\w\d]+@[\w\d]+(\.[\w\d]+)+$/.test(value),
    login: (value) => value?.length >= 3 && value?.length <= 255,
    name: (value) => value?.length === 0 || value?.length >= 3 && value?.length <= 255,
    password: (value) => value?.length > 5 && value?.length <= 255,
    desc: (value) => value?.length === 0 || value?.length >= 5 && value?.length <= 5000
}

const Validation = {
    Auth,
    Video,
    User
}

export default Validation;