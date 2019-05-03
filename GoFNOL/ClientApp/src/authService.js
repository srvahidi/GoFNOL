import { UserManager } from 'oidc-client'

let user

let userManager

let disableAuth

const setUser = (newUser) => {
	user = newUser

	if (user) {
		userManager.startSilentRenew()
	}
}

export const createAuthService = async (config) => {
	disableAuth = config.disableAuth
	if (disableAuth) {
		return
	}

	const location = window.location
	const appUri = `${location.protocol}//${location.host}`
	const redirectUri = `${appUri}/auth-callback`
	const silentRedirectUri = `${appUri}/auth-silent-callback`
	const oidcConfig = {
		authority: config.isEndpoint,
		client_id: 'gofnol-ui',
		redirect_uri: redirectUri,
		response_type: 'id_token token',
		scope: 'openid user.organization gofnol.api',
		silent_redirect_uri: silentRedirectUri
	}

	userManager = new UserManager(oidcConfig)
	setUser(await userManager.getUser())
}

export const getUserName = () => disableAuth ? 'f00generic1' : user && user.profile.sub

export const getRequestHeaders = () => {
	if (disableAuth) {
		return { 'org-id': 'F00' }
	}

	return user && {
		Authorization: `${user.token_type} ${user.access_token}`,
		'org-id': JSON.parse(user.profile['user.organization']).ExternalId
	}
}

export const isSignedIn = () => disableAuth ? true : user && !user.expired

export const signIn = () => {
	if (disableAuth) {
		return
	}

	userManager.signinRedirect()
}

export const signOut = () => {
	if (disableAuth) {
		return
	}

	userManager.signoutRedirect()
}

export const completeSignIn = async () => {
	if (disableAuth) {
		return
	}

	setUser(await userManager.signinRedirectCallback())
}

export const completeSilentSignIn = () => {
	if (disableAuth) {
		return
	}

	this.userManager.signinSilentCallback()
}
