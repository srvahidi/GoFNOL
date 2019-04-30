import { UserManager } from 'oidc-client'

class AuthService {

	userManager

	user

	initialize = (isEndpoint) => {
		const redirectUri = `${window.location.href}auth-callback`
		const oidcConfig = {
			authority: isEndpoint,
			client_id: 'gofnol-ui',
			redirect_uri: redirectUri,
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
	}

	getUserName = () => this.user && this.user.profile.sub

	getRequestHeaders = () => this.user && {
		'org-id': JSON.parse(this.user.profile['user.organization']).ExternalId,
		'Authorization': `${this.user.token_type} ${this.user.access_token}`
	}
}

export const AuthServiceInstance = new AuthService()
