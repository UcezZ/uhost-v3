/**
 * 
 * @param {*} user 
 * @returns {Number[]}
 */
function getUserRightIds(user) {
    var rightsIds = user?.rights?.map
        && user?.rights?.filter
        && user.rights.filter(e => e?.id).map(e => e?.id);

    return rightsIds ?? [];
}

/**
 * Checks if user has any of rights
 * @param {*} user Current user from state context
 * @param {...number} rights Rights to check
 * @returns {Boolean}
 */
export function checkAnyRight(user, ...rights) {
    if (!rights?.length) {
        return false;
    }

    var userRights = getUserRightIds(user);

    if (!userRights?.length) {
        return false;
    }

    return rights.some(e => userRights.includes(e));
}

/**
 * Checks if user has all of rights
 * @param {*} user Current user from state context
 * @param {...number} rights Rights to check
 * @returns {Boolean}
 */
export function checkAllRights(user, ...rights) {
    if (!rights?.length) {
        return false;
    }

    var userRights = getUserRightIds(user);

    if (!userRights?.length) {
        return false;
    }

    return rights.every(e => userRights.includes(e));
}

const Rights = {
    VideoCreateUpdate: 0x01,
    VideoDelete: 0x02,
    VideoGetAll: 0x03,
    FileGet: 0x11,
    FileCreateUpdate: 0x12,
    FileDelete: 0x13,
    checkAllRights,
    checkAnyRight
}

export default Rights;