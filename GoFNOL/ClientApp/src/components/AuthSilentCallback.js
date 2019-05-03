import { Component } from 'react'

import { completeSilentSignIn } from '../authService'

export class AuthSilentCallback extends Component {
	async componentDidMount() {
		await completeSilentSignIn()
	}

	render() {
		return null
	}
}