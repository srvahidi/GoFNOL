import { Component } from 'react'

export class AuthSilentCallback extends Component {
	async componentDidMount() {
		await this.props.authService.completeSilentSignIn()
	}

	render() {
		return null
	}
}