import { UserManager } from 'oidc-client'

class AuthService {
	initialize = (isEndpoint) => {
		const location = window.location
		const redirectUri = `${location.protocol}//${location.host}/auth-callback`
		const silentRedirectUri = `${location.protocol}//${location.host}/auth-silent-callback`
		const oidcConfig = {
			authority: isEndpoint,
			client_id: 'gofnol-ui',
			redirect_uri: redirectUri,
			silent_redirect_uri: silentRedirectUri,
			response_type: 'id_token token',
			scope: 'openid user.organization gofnol.api'
		}
		this.userManager = new UserManager(oidcConfig)
	}

	isSignedIn = () => !!this.user

	signIn = () => {
		this.userManager.signinRedirect()
	}

	signOut = () => {
		this.userManager.signoutRedirect()
	}

	completeSignIn = async () => {
		this.user = await this.userManager.signinRedirectCallback()
		this.userManager.startSilentRenew()
	}

	completeSilentSignIn = () => {
		this.userManager.signinSilentCallback()
	}

	getUserName = () => this.user && this.user.profile.sub

	getRequestHeaders = () => this.user && {
		'org-id': JSON.parse(this.user.profile['user.organization']).ExternalId,
		'Authorization': `${this.user.token_type} ${this.user.access_token}`
	}
}

export const AuthServiceInstance = new AuthService()
